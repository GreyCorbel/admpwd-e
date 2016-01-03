using System;
using System.Collections.Generic;

namespace AdmPwd.PDS.KeyStore
{
    public interface IKeyStore
    {
        List<int> SupportedKeySizes { get; }

        Dictionary<UInt32, string> PublicKeys { get; }

        string Decrypt(UInt32 keyID, string EncryptedPwd);

        UInt32 GenerateKeyPair(int KeySize);

        string GetPublicKey(UInt32 KeyID);

    }
}
