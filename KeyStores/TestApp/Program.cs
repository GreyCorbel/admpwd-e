using System;
using System.Collections.Generic;
using AdmPwd.PDS.KeyStore;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AzureKeyVaultStore store = new AzureKeyVaultStore();
			
			//initialize the keystore
			//0 means basic functionality; 1 means more functionality, etc.
			//functionality levels are defined by the vendor of keystore implementation
            store.Initialize(0);

            var sizes = store.SupportedKeySizes;

            store.GenerateKeyPair(sizes[0]);

            var pubKeys = store.GetPublicKeys();

            string secret = "This is secure";

            var encryptedSecret = store.Encrypt(0, secret);

            string[] parts = encryptedSecret.Split(new string[] { ": " }, StringSplitOptions.None);

            uint keyId = uint.Parse(parts[0]);

            string decryptedSecret = store.Decrypt(keyId, parts[1]);
        }
    }
}
