#This script analyzes permissions to Read/Reset/Uplad assword granted by LAPS PS module (Set-AdmPwdReadPasswordPermission, Set-AdmwdResetPasswordPermission, Set-AdmPwdComputerSelfPermission)
#Optionally, script also removes permissions found for given user or group
#Script analyzes delegated non-inherited permissions (does not show permissions inherited from upper level container)

#Script is suitable for review of eisting permissions, or for removal of permissions in case you want to upgrade from L"APS to AdmPwd.E
#Note: Script is specialized to work with LAPS permisions; AdmPwd.E permission model uses diferent permissions
#  (only AdmPwd.E PDS permissions may be detected as ReadPassword permission by this script, so care must be taken when cleaning permissions so as not to break AdmPwd.E PDS permissions)

#Usage:
#Review permissions on a subtree (containers only):
#   LapsCleanup.ps1 -DN 'ou=myOU,dc=myDomain,dc=com' -ShowProgress
#Review permissions on a subtree (containers and computer accounts):
#   LapsCleanup.ps1 -DN 'ou=myOU,dc=myDomain,dc=com' -ShowProgress -IncludeComputers
#Review permissions on a subtree (containers only) and remove all LAPS permissions for account MyDomain\MyUser:
#   LapsCleanup.ps1 -DN 'ou=myOU,dc=myDomain,dc=com' -ShowProgress -PrincipalToRemove MyDomain\MyUser


Param
(
    [Parameter(Mandatory=$true)]
    [string]$DN,
    [Switch]$IncludeComputers,
    [string]$PrincipalToRemove,
    [Switch]$ShowProgress
)

#relies on S.DS.P v1.8.6
Import-Module S.DS.P -RequiredVersion 1.8.6

#Add type describing access level
Add-Type @'
public enum AccessType
{
    ReadPassword,
    WritePassword,
    ResetPassword
}

public enum OperationResult
{
    Preserved,
    Removed
}

public class LAPSPermission
{
    public string DistinguishedName {get; set;}
    public System.Security.Principal.IdentityReference Principal {get; set;}
    public AccessType Access {get; set;}
    public OperationResult Result {get; set;}

    public LAPSPermission(string dn, System.Security.Principal.IdentityReference principal, AccessType access, OperationResult result=OperationResult.Preserved)
    {
        this.DistinguishedName = dn;
        this.Principal = principal;
        this.Access = access;
        this.Result=result;
    }
}
'@

#get LDAP connection for current domain
$conn = Get-LdapConnection

#get RootDSE - we need it to know schema partition DN
$dse = Get-RootDSe -LdapConnection $conn

#get IDs of schema attributes and classes. Note that LAPS attributes schema IDs are different in each AD forest
$pwdGuid=new-object Guid -ArgumentList (,[byte[]](Find-LdapObject -LdapConnection $conn -searchFilter '(&(objectClass=attributeSchema)(cn=ms-Mcs-AdmPwd))' -searchBase $dse.SchemaNamingContext -searchScope OneLevel -PropertiesToLoad 'schemaIdGUid' -BinaryProperties 'schemaIdGuid').schemaIdGuid)
$pwdExpirationGuid = new-object Guid -ArgumentList (,[byte[]](Find-LdapObject -LdapConnection $conn -searchFilter '(&(objectClass=attributeSchema)(cn=ms-Mcs-AdmPwdExpirationTime))' -searchBase $dse.SchemaNamingContext -searchScope OneLevel -PropertiesToLoad 'schemaIdGUid' -BinaryProperties 'schemaIdGuid').schemaIdGuid)
$computerGuid = new-object Guid -ArgumentList (,[byte[]](Find-LdapObject -LdapConnection $conn -searchFilter '(&(objectClass=classSchema)(cn=computer))' -searchBase $dse.SchemaNamingContext -searchScope OneLevel -PropertiesToLoad 'schemaIdGUid' -BinaryProperties 'schemaIdGuid').schemaIdGuid)

#Directory control that instructs to load DACL when retrieving directory object ACL
$ctrl = new-object System.DirectoryServices.Protocols.SecurityDescriptorFlagControl([System.DirectoryServices.Protocols.SecurityMasks]::Dacl)

if(-not [string]::IsNullOrEmpty($PrincipalToRemove))
{
    $toRemove = new-object System.Security.Principal.NTAccount($PrincipalToRemove)
}
else {
    $toRemove=$null
}
#get containers in subtree under given search root
#Note: this needs to run elevated
$Containers = @(Find-ldapObject -LdapConnection $conn -searchFilter '(|(objectCategory=container)(objectCategory=organizationalUnit))' -searchBase $dn -PropertiesToLoad 'ntSecurityDescriptor' -binaryProperties 'ntSecurityDescriptor'  -AdditionalControls $ctrl)

#analyze permissions on each container
foreach($container in $Containers)
{
    if($ShowProgress)
    {
        Write-Progress -Activity 'Analyzing ACL' -Status 'Container' -CurrentOperation $container.distinguishedName
    }
    #load ACL on container
    #$obj = Find-LdapObject -LdapConnection $conn -searchFilter '(objectClass=*)' -searchBase $container.distinguishedName -searchScope Base -PropertiesToLoad 'ntSecurityDescriptor' -binaryProperties 'ntSecurityDescriptor'  -AdditionalControls $ctrl
    $acl=new-object System.DirectoryServices.ActiveDirectorySecurity
    $acl.SetSecurityDescriptorBinaryForm($container.ntSecurityDescriptor)

    #array of ACEs to be removed from ACL - those with matching PrincipalToRemove
    $acesToRemove=@()
    foreach($ace in $ACL.GetAccessRules($true,$false,[System.Security.Principal.NTAccount]))
    {
        #LAPS permissions delegated to container inherit to computer  objects only
        if($ace.InheritedObjectType -eq $computerGuid)
        {
            if($ace.ObjectType -eq $pwdGuid -and $ace.ActiveDirectoryRights -eq ([System.DirectoryServices.ActiveDirectoryRights]::ReadProperty -bor [System.DirectoryServices.ActiveDirectoryRights]::ExtendedRight))
            {
                #we have rule that allows password reading - send match to pipeline and schedule for removal if match for PrincipalToRemove found
                if($ace.IdentityReference.Equals($toRemove)) {
                    $acesToRemove+=$ace
                    new-object LAPSPermission($container.distinguishedName, $ace.IdentityReference, [AccessType]::ReadPassword, [OperationResult]::Removed)
                } else {
                    new-object LAPSPermission($container.distinguishedName, $ace.IdentityReference, [AccessType]::ReadPassword)
                }
                continue;
            }
            if($ace.ObjectType -eq $pwdExpirationGuid -and ($ace.ActiveDirectoryRights -band [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty) -eq [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty)
            {
                #we have rule that allows password reset - send match to pipeline and schedule for removal if match for PrincipalToRemove found
                if($ace.IdentityReference.Equals($toRemove)) {
                    $acesToRemove+=$ace
                    new-object LAPSPermission($container.distinguishedName, $ace.IdentityReference, [AccessType]::ResetPassword, [OperationResult]::Removed)
                }
                else {
                    new-object LAPSPermission($container.distinguishedName, $ace.IdentityReference, [AccessType]::ResetPassword)
                }
               continue;
            }
    
            if($ace.ObjectType -eq $pwdGuid -and ($ace.ActiveDirectoryRights -band [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty) -eq [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty)
            {
                #we have rule that allows password update - typically granted to SELF - send match to pipeline and schedule for removal if match for PrincipalToRemove found
                if($ace.IdentityReference.Equals($toRemove))
                {
                    $acesToRemove+=$ace
                    new-object LAPSPermission($container.distinguishedName, $ace.IdentityReference, [AccessType]::WritePassword, [OperationResult]::Removed)
                }
                else {
                    new-object LAPSPermission($container.distinguishedName, $ace.IdentityReference, [AccessType]::WritePassword)
                }
                continue;
            }
        }
    }

    #remove ACEs from ACL
    foreach($ace in $acesToRemove)
    {
        $acl.RemoveAccessRule($ace) | Out-Null
    }
    if($acesToRemove.count -gt 0)
    {
        $container.ntSecurityDescriptor = @(,$acl.GetSecurityDescriptorBinaryForm())
        Edit-LdapObject -Object $container -LdapConnection $conn -AdditionalControls $ctrl -IncludedProps 'ntSecurityDescriptor' -BinaryProps 'ntSecurityDescriptor'
    }

    #process computer accounts as well, if required
    if($IncludeComputers)
    {
        #load computer objects in container
        #Note: this needs to run elevated
        $compsToAnalyze = @(Find-ldapObject -LdapConnection $conn -searchFilter '(objectClass=computer)' -searchBase $container.distinguishedName -searchScope OneLevel -PropertiesToLoad 'ntSecurityDescriptor' -binaryProperties 'ntSecurityDescriptor'  -AdditionalControls $ctrl)
        foreach($comp in $compsToAnalyze)
        {
            if($ShowProgress)
            {
                Write-Progress -Activity 'Analyzing ACL' -Status 'ComputersInContainer' -CurrentOperation $comp.distinguishedName
            }
            $acl=new-object System.DirectoryServices.ActiveDirectorySecurity
            $acl.SetSecurityDescriptorBinaryForm($comp.ntSecurityDescriptor)

            $acesToRemove=@()
            foreach($ace in $ACL.GetAccessRules($true,$false,[System.Security.Principal.NTAccount]))
            {
                if($ace.ObjectType -eq $pwdGuid -and $ace.ActiveDirectoryRights -eq ([System.DirectoryServices.ActiveDirectoryRights]::ReadProperty -bor [System.DirectoryServices.ActiveDirectoryRights]::ExtendedRight))
                {
                    #we have rule that allows password reading
                    if($ace.IdentityReference.Equals($toRemove)) {
                        $acesToRemove+=$ace
                        new-object LAPSPermission($comp.distinguishedName, $ace.IdentityReference, [AccessType]::ReadPassword, [OperationResult]::Removed)
                    }
                    else {
                        new-object LAPSPermission($comp.distinguishedName, $ace.IdentityReference, [AccessType]::ReadPassword)
                    }
                    continue;
                }
                if($ace.ObjectType -eq $pwdExpirationGuid -and ($ace.ActiveDirectoryRights -band [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty) -eq [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty)
                {
                    #we have rule that allows password reset
                    if($ace.IdentityReference.Equals($toRemove))
                    {
                        $acesToRemove+=$ace
                        new-object LAPSPermission($comp.distinguishedName, $ace.IdentityReference, [AccessType]::ResetPassword, [OperationResult]::Removed)
                    }
                    else {
                        new-object LAPSPermission($comp.distinguishedName, $ace.IdentityReference, [AccessType]::ResetPassword)
                    }
                    continue;
                }
                if($ace.ObjectType -eq $pwdGuid -and ($ace.ActiveDirectoryRights -band [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty) -eq [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty)
                {
                    #we have rule that allows password update - typically granted to SELF
                    if($ace.IdentityReference.Equals($toRemove))
                    {
                        $acesToRemove+=$ace
                        new-object LAPSPermission($comp.distinguishedName, $ace.IdentityReference, [AccessType]::WritePassword, [OperationResult]::Removed)
                    }
                    new-object LAPSPermission($comp.distinguishedName, $ace.IdentityReference, [AccessType]::WritePassword)
                    continue;
                }
            }

            foreach($ace in $acesToRemove)
            {
                $acl.RemoveAccessRule($ace) | Out-Null
            }
            if($acesToRemove.count -gt 0)
            {
                $comp.ntSecurityDescriptor = @(,$acl.GetSecurityDescriptorBinaryForm())
                Edit-LdapObject -Object $comp -LdapConnection $conn -AdditionalControls $ctrl -IncludedProps 'ntSecurityDescriptor' -BinaryProps 'ntSecurityDescriptor'
            }
        }
    }
}
