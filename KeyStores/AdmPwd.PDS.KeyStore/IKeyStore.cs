using System;
using System.Collections.Generic;

namespace AdmPwd.PDS.KeyStore
{
    /// <summary>
    /// Interface for implementation of AdmPwd.E PDS KeyStore
    /// </summary>
    public interface IKeyStore
    {
        /// <summary>
        /// Returns list of RSA key sizes supported by keystore.
        /// Purpose of this is to limit sizes of RSA keys that keystore can produce
        /// This does not limit capability of keystore to use key for Decrypt opration - this means it can fddecpryt using key whit unsupported size
        /// </summary>
        List<int> SupportedKeySizes { get; }

        /// <summary>
        /// Returns list of all public keys maintained by the keystore
        /// </summary>
        Dictionary<UInt32, string> PublicKeys { get; }

        /// <summary>
        /// Decrypts secret using key with given keyID
        /// </summary>
        /// <param name="keyID">ID of the key to use for decryption</param>
        /// <param name="EncryptedPwd">Encrypted secret to decrypt, encoded as Base64 string</param>
        /// <returns></returns>
        string Decrypt(UInt32 keyID, string EncryptedPwd);

        /// <summary>
        /// Generates new RSA KeyPair
        /// </summary>
        /// <param name="KeySize">
        /// Size of new RSA key, in bits. Size must be one of <see cref="SupportedKeySizes"/> 
        /// </param>
        /// <returns></returns>
        UInt32 GenerateKeyPair(int KeySize);

        /// <summary>
        /// Returns public key with specified KeyID, or null if key with given KeyID is not managed by KeyStore.
        /// Key is returned as Base64 encoded blob
        /// </summary>
        /// <param name="KeyID">ID of the key to return</param>
        /// <returns></returns>
        string GetPublicKey(UInt32 KeyID);

    }
}
