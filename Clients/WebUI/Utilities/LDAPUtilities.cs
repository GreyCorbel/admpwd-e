using System;
using System.Text;
using System.Collections.Generic;
using AdmPwd.Types;
//using AdmPwd.Utils;
using System.DirectoryServices.Protocols;

namespace AdmPwd.Portal.Utilities
{
    public static class LDAPUtilities
    {
        static LDAPUtilities()
        {
        }
        //public static SearchResultEntry GetComputerObj(string computerName)
        //{
        //    SearchResultEntry result = null;
        //    List<string> computerDNs = AdmPwd.Utils.DirectoryUtils.GetComputerDN(computerName);

        //    // not found
        //    if (computerDNs.Count == 0)
        //        throw new ADNoComputerFoundException(computerName);
        //    // more then 1 computer found
        //    if (computerDNs.Count > 1)
        //    {
        //        StringBuilder computers = new StringBuilder();
        //        foreach (string cDN in DirectoryUtils.GetComputerDN(computerName))
        //            computers.Append(cDN + Environment.NewLine);
        //        throw new ADComputerObjectAmbiguousException(computers.ToString());
        //    }
        //    // just 1 computer DN found, get additional properties
        //    string computerDN = computerDNs[0];

        //    using (LdapConnection connectionLDAP = DirectoryUtils.GetLdapConnection(ConnectionType.LDAP))
        //    {
        //        SearchRequest request = new SearchRequest(computerDN,
        //                                        String.Format("(&(objectCategory=computer)(distinguishedName={0}))", computerDN),
        //                                        System.DirectoryServices.Protocols.SearchScope.Base,
        //                                        new string[] { "distinguishedName", "ntSecurityDescriptor", Constants.PasswordAttributeName, Constants.TimestampAttributeName, Constants.PasswordHistoryAttributeName });
        //        request.Controls.Add(new SecurityDescriptorFlagControl(System.DirectoryServices.Protocols.SecurityMasks.Dacl));

        //        SearchResponse response = null;
        //        try { response = (SearchResponse)connectionLDAP.SendRequest(request); }
        //        catch (LdapException ex) { throw new LDAPNotAvailable("LDAP: " + ex.Message); }
        //        if (response.Entries.Count == 0)
        //            throw new ADNoComputerFoundException(" DN: " + computerDN);
        //        result = response.Entries[0];
        //    }

        //    return result;
        //}

        //public static LDAPUserInfo GetUserInfo(string userDomain, string userName)
        //{
        //    SearchResultEntry entry = GetUser(userDomain, userName);
        //    LDAPUserInfo userInfo = new LDAPUserInfo();
        //    userInfo.UserID = userName;
        //    userInfo.FullName = GetAttributeValue<string>(entry, "cn");
        //    userInfo.Domain = userDomain;
        //    userInfo.Email = GetAttributeValue<string>(entry, "mail");
        //    userInfo.CountryOU = GetAttributeValue<string>(entry, "OU");
        //    userInfo.Phone = GetAttributeValue<string>(entry, "telephoneNumber");
        //    userInfo.Mobile = GetAttributeValue<string>(entry, "mobile");
        //    return userInfo;
        //}

        //public static SearchResultEntry GetUser(string forestDnsName, string userName)
        //{
        //    SearchResultEntry result = null;
        //    //AdmPwd.Types.ForestInfo forest = AdmPwd.Utils.DirectoryUtils.GetForestRootDomain();
        //    string userDN = string.Empty;
        //    using (LdapConnection connection = AdmPwd.Utils.DirectoryUtils.GetLdapConnection(forestDnsName, AdmPwd.Utils.ConnectionType.GC))
        //    {
        //        SearchRequest request = new SearchRequest(null, //looking in every domain partition
        //                                        string.Format("(&(objectCategory=user)(sAMAccountName={0}))", userName),
        //                                        System.DirectoryServices.Protocols.SearchScope.Subtree,
        //                                        new string[] {"1.1"}    //not interested in atributes, just want distinguishedName
        //                                        );

        //        SearchResponse response = (SearchResponse)connection.SendRequest(request);

        //        if (response.Entries.Count == 0)
        //            throw new LDAPNoEntryFoundException();
        //        else if (response.Entries.Count > 1)
        //            throw new LDAPMoreThanOneEntryFoundException();

        //        // user found, get additional properties
        //        userDN = response.Entries[0].DistinguishedName;
        //    }

        //    using (LdapConnection connectionLDAP = AdmPwd.Utils.DirectoryUtils.GetLdapConnection(forestDnsName, AdmPwd.Utils.ConnectionType.LDAP))
        //    {
        //        SearchRequest searchRequest = new SearchRequest(userDN,
        //                                            "(objectCategory=user)",
        //                                            System.DirectoryServices.Protocols.SearchScope.Base,
        //                                        new string[] { "cn", "mail", "OU", "telephoneNumber", "mobile", "tokenGroups", "objectSid" });

        //        SearchResponse searchResponse = (SearchResponse)connectionLDAP.SendRequest(searchRequest);

        //        if (searchResponse.Entries.Count == 0)
        //            throw new LDAPNoEntryFoundException();
        //        else if (searchResponse.Entries.Count > 1)
        //            throw new LDAPMoreThanOneEntryFoundException();

        //        result = searchResponse.Entries[0];
        //    }

        //    return result;
        //}

        //public static T GetAttributeValue<T>(SearchResultEntry searchResult, string attributeName)
        //{
        //    DirectoryAttribute fullName = searchResult.Attributes[attributeName];
        //    if (fullName == null)
        //        return default(T);

        //    object[] attributeValues = fullName.GetValues(typeof(string));
        //    if (attributeValues != null && attributeValues.Length == 1)
        //        return (T)attributeValues[0];

        //    return default(T);
        //}

        //private static DateTime? GetAttributeValueDateTime(SearchResultEntry searchResult, string attributeName)
        //{
        //    string timestamp = searchResult.Attributes[attributeName.ToLower()].GetValues(typeof(string))[0] as string;
        //    if (!string.IsNullOrEmpty(timestamp))
        //        return DateTime.FromFileTime(long.Parse(timestamp));
        //    else
        //        return null;
        //}

        //private static string GetAttributeValueString(SearchResultEntry searchResult, string attributeName)
        //{
        //    DirectoryAttribute attr = searchResult.Attributes[attributeName];
        //    if (attr == null) return string.Empty;

        //    object[] attributeValues = attr.GetValues(typeof(string));
        //    if (attributeValues != null && attributeValues.Length == 1)
        //        return (string)attributeValues[0];

        //    return string.Empty;
        //}
    }
    //public class LDAPNotAvailable : Exception
    //{
    //    public LDAPNotAvailable() { }
    //    public LDAPNotAvailable(string message) : base(message) { }
    //}

    //public class LDAPNoEntryFoundException : Exception
    //{
    //    public LDAPNoEntryFoundException() { }
    //    public LDAPNoEntryFoundException(string message) : base(message) { }
    //}

    //public class LDAPMoreThanOneEntryFoundException : Exception
    //{
    //    public LDAPMoreThanOneEntryFoundException() { }
    //    public LDAPMoreThanOneEntryFoundException(string message) : base(message) { }
    //}
    //public class ADComputerObjectAmbiguousException : Exception
    //{
    //    public ADComputerObjectAmbiguousException() { }
    //    public ADComputerObjectAmbiguousException(string message) : base(message) { }
    //}

    //public class ADNoComputerFoundException : Exception
    //{
    //    public ADNoComputerFoundException() { }
    //    public ADNoComputerFoundException(string message) : base(message) { }
    //}

}