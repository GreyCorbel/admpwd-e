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

            var sizes = store.SupportedKeySizes;

            store.GenerateKeyPair(sizes[0]);

            var pubKeys = store.GetPublicKeys();
        }
    }
}
