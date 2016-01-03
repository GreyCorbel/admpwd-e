using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdmPwd.PDS.AzureKeyStore
{
    public class KeyPair
    {
        protected KeyData _pubKey;
        protected KeyData _privKey;

        public KeyPair(KeyData PublicKey, KeyData PrivateKey)
        {
            _pubKey = PublicKey;
            _privKey = PrivateKey;
        }

        public KeyData PublicKey
        {
            get
            {
                return _pubKey;
            }
        }

        public KeyData PrivateKey
        {
            get
            {
                return _privKey;
            }
        }
    }
}