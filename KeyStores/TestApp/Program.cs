using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmPwd.PDS.AzureKeyStore;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AzureKeyVaultStore store = new AzureKeyVaultStore();

            var sizes = store.SupportedKeySizes;

            store.GenerateKeyPair(sizes[0]);

            var pubKeys = store.PublicKeys;
        }
    }
}
