using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmPwd.PDS.KeyStore
{
    /// <summary>
    /// Descriptor of key maintained by implementation of Keystore
    /// </summary>
    public class KeyData
    {
        /// <summary>
        /// ID of the key. Lowest allowed ID is 1
        /// </summary>
        public uint ID;

        /// <summary>
        /// Type of the key. Currently supported values are CryptoAPI_RSA and CNG_RSA
        /// String type chosen for simplicity and extensibility of interface - we did not want to introduce enum here
        /// </summary>
        public string Type;

        /// <summary>
        /// Size of key in bits
        /// </summary>
        public int Size;

        /// <summary>
        /// Key blob, encoded as Base64 string. Includes metadata used by clients.
        /// </summary>
        public string Key;
    }
}
