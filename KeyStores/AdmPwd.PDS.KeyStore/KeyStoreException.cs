using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmPwd.PDS.KeyStore
{
    /// <summary>
    /// Base exception thrown by implementation of KeyStore
    /// </summary>
    public class KeyStoreException:Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message"></param>
        public KeyStoreException(string message):base(message)
        {

        }
    }
}
