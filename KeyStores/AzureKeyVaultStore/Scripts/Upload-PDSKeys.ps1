Param
(
	[String]$folder="D:\Keys",
	[String]$Vault="LAPS",
	[String]$Area="DEV",
    [UInt32]$StartID=1,
    [UInt32]$StopID=1
)

$KeyID=$StartID
while($true)
{
	$pubKeyFile="$folder\$KeyID`_PublicKey`.dat"
	$privKeyFile="$folder\$KeyID`_PrivateKey`.dat"

	if(((-not [System.IO.FIle]::Exists($pubKeyFile)) -and (-not [System.IO.FIle]::Exists($privKeyFile))) -or ($KeyID -gt $StopID))
	{
		break;
	}
	$pubKey=$null
	if([System.IO.FIle]::Exists($pubKeyFile))
	{
		$pubKey=[System.IO.File]::ReadAllText($pubKeyFile)
	}
	$privKey=$null
	if([System.IO.FIle]::Exists($privKeyFile))
	{
		$privKey=[System.IO.File]::ReadAllText($privKeyFile)
	}

	if($pubKey -ne $null -and $privKey -ne $null)
	{
		$tags=@{KeyID="$KeyID";Area=$Area;KeyType="Public"}
		$Key=ConvertTo-SecureString -String $pubKey -AsPlainText -Force

		Set-AzureKeyVaultSecret -VaultName $Vault -Name ([Guid]::NewGuid().ToString()) -Tags $tags -SecretValue $Key

		$tags=@{KeyID="$KeyID";Area=$Area;KeyType="Private"}
		$Key=ConvertTo-SecureString -String $privKey -AsPlainText -Force

		Set-AzureKeyVaultSecret -VaultName $Vault -Name ([Guid]::NewGuid().ToString()) -Tags $tags -SecretValue $Key

	}
    $KeyID++
}
