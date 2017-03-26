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
        /// This does not limit capability of keystore to use key for Decrypt operation - this means it can decpryt using key with unsupported size
        /// </summary>
        List<int> SupportedKeySizes { get; }

        /// <summary>
        /// Decrypts secret using key with given keyID
        /// </summary>
        /// <param name="keyID">ID of the key to use for decryption</param>
        /// <param name="EncryptedPwd">Encrypted secret to decrypt, encoded as Base64 string</param>
        /// <returns>
        /// Decrypted data as string
        /// </returns>
        string Decrypt(UInt32 keyID, string EncryptedPwd);

        /// <summary>
        /// Generates new KeyPair
        /// </summary>
        /// <param name="KeySize">
        /// Size of new key, in bits. Size must be one of <see cref="SupportedKeySizes"/>, otherwise keystore must refuse the operation and throw <see cref="KeyStoreException"/>
        /// </param>
        /// <returns>
        /// ID of newly generated key pair
        /// </returns>
        UInt32 GenerateKeyPair(int KeySize);

        /// <summary>
        /// Returns public key with specified KeyID, or null if key with given KeyID is not managed by KeyStore.
        /// </summary>
        /// <param name="KeyID">ID of the key to return</param>
        /// <returns>
        /// <see cref="KeyData"/> describing public key
        /// </returns>
        KeyData GetPublicKey(UInt32 KeyID);

        /// <summary>
        /// Returns all public keys managed by a keystore
        /// </summary>
        /// <returns>
        /// List of <see cref="KeyData"/> objects describing public keys managed by implementation of KeyStore
        /// </returns>
        List<KeyData> GetPublicKeys();

        /// <summary>
        /// Used to carry on initialization tasks necessary for keystore
        /// </summary>
        /// <param name="FunctionalityLevel">Identifies desired functionality level of keystore. Keystore may implement various functionality level - some of them for free, some paid.
        /// PDS can read license information for keystore and pass desired functionality level to keystore, according to license provided.
        /// </param>
        void Initialize(int FunctionalityLevel);

    }
}
