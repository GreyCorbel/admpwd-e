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
        }
    }
}
