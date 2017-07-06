Param
(
	[String]$folder="D:\Keys",
	[String]$Vault="AdmPwd.E",
	[String]$Area="DEV",
    [UInt32]$StartID=1,
    [UInt32]$StopID=1
)

#Note: AzureKeyVaultStore implementation now only supports RSA_CryptoAPI keys. Support for RSA_Cng keys is coming soon
$KeyID=$StartID
while($true)
{
	if($KeyID -gt $StopID)
	{
		break;
	}
	$KeyFile="$folder\$KeyID`_Key`.dat"

	if(((-not [System.IO.FIle]::Exists($KeyFile))) -or ($KeyID -gt $StopID))
	{
		break;
	}
	$Key=$null
	if([System.IO.FIle]::Exists($KeyFile))
	{
		$Key=[System.IO.File]::ReadAllText($KeyFile)
	}

	if($Key -ne $null)
	{
		$tags=@{KeyID="$KeyID";Area=$Area}
		$Key=ConvertTo-SecureString -String $Key -AsPlainText -Force

		Set-AzureKeyVaultSecret -VaultName $Vault -Name ([Guid]::NewGuid().ToString()) -Tags $tags -SecretValue $Key
	}
    $KeyID++
}
