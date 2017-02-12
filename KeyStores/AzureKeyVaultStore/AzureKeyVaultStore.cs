using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Configuration;
using System.Security.Cryptography;
using AdmPwd.PDS.KeyStore;

namespace AdmPwd.PDS.AzureKeyStore
{
    public class AzureKeyVaultStore : IKeyStore
    {

        #region Protected members
        /// <summary>
        /// vault uri, such as https://laps.vault.azure.net/
        /// Notes:
        ///     Rememebr to include trailing slash
        /// </summary>
        protected Uri _vaultUri;

        /// <summary>
        /// application ID, as created by AAD admin, such as "9873ac72-d7ed-4443-bd44-123b9247d6ed"
        /// </summary>
        protected string _clientId;

        /// <summary>
        /// application authorization key, as generated when creating application definition in AAD
        /// </summary>
        protected string _appKey;

        /// <summary>
        /// identifier of aad instance, such as https://login.windows.net/formacek.com
        /// </summary>
        protected string _aadInstance;

        /// <summary>
        /// This specifies subset of keys KeyStore operates with
        /// KeyVault may contain multiple sets of keys, differentiated by "Area" tag
        /// if area is not specified, all keys in KeyVault are processed by an instance of KeyStore
        /// </summary>
        protected string _area;

        List<int> _keySizes = new List<int>();

        #endregion

        #region Constants
        /// <summary>
        /// AAD Api version to use
        /// </summary>
        protected string _apiVersion = "2016-10-01";
        /// <summary>
        /// resource that receives REST API calls
        /// </summary>
        /// 
        protected string resource = "https://vault.azure.net";

        #endregion

        private Dictionary<UInt32, VaultKeyData> _keys = new Dictionary<UInt32, VaultKeyData>();

        public AzureKeyVaultStore()
        {

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AzureKeyVaultStore:VaultUri"])
                || string.IsNullOrEmpty(ConfigurationManager.AppSettings["AzureKeyVaultStore:AADInstance"])
                || string.IsNullOrEmpty(ConfigurationManager.AppSettings["AzureKeyVaultStore:AccessKey"])
                || string.IsNullOrEmpty(ConfigurationManager.AppSettings["AzureKeyVaultStore:ClientId"])
            )
                throw new ConfigurationErrorsException("Please verify configuration file and provide values for AzureKeyVaultStore:* settings");

            _vaultUri = new Uri(ConfigurationManager.AppSettings["AzureKeyVaultStore:VaultUri"]);
            _aadInstance = ConfigurationManager.AppSettings["AzureKeyVaultStore:AADInstance"];
            _appKey = ConfigurationManager.AppSettings["AzureKeyVaultStore:AccessKey"];
            _clientId = ConfigurationManager.AppSettings["AzureKeyVaultStore:ClientId"];
            _area = ConfigurationManager.AppSettings["AzureKeyVaultStore:Area"];

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AzureKeyVaultStore:SupportedKeySizes"]))
            {
                string[] values = ConfigurationManager.AppSettings["AzureKeyVaultStore:SupportedKeySizes"].Split(',');
                foreach (var value in values)
                {
                    int val = 0;
                    int.TryParse(value, out val);
                    if (val > 0)
                        _keySizes.Add(val);
                }
            }

            LoadKeys().Wait();
        }

        protected async Task LoadKeys()
        {
            AuthenticationResult result = await Authenticate();

            List<VaultKeyData> keys = new List<VaultKeyData>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                UriBuilder ub = new UriBuilder(_vaultUri);
                ub.Path += "secrets";
                ub.Query = "api-version=" + _apiVersion + "&maxresults=25"; //maxresults seems to be required by API

                HttpResponseMessage response = await client.GetAsync(ub.Uri);
                if (!response.IsSuccessStatusCode)
                    throw new KeyStoreException(response.ReasonPhrase);
                bool isAtEnd = false;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SecretList.SecretList));
                DataContractJsonSerializer serializer2 = new DataContractJsonSerializer(typeof(Secret.Secret));
                do
                {
                    SecretList.SecretList secrets = null;
                    using (var data = await response.Content.ReadAsStreamAsync())
                    {
                        secrets = (SecretList.SecretList)serializer.ReadObject(data);
                    }
                    if (secrets.value != null)
                    {
                        foreach (var secret in secrets.value)
                        {
                            UriBuilder ub2 = new UriBuilder(secret.id);
                            ub2.Query = "api-version=" + _apiVersion;
                            response = await client.GetAsync(ub2.Uri);
                            if (!response.IsSuccessStatusCode)
                                throw new KeyStoreException(response.ReasonPhrase);

                            using (var details = await response.Content.ReadAsStreamAsync())
                            {
                                Secret.Secret sec = (Secret.Secret)serializer2.ReadObject(details);
                                if (_area != null && string.Compare(_area, sec.tags.Area, true) != 0)
                                    continue;

                                VaultKeyData key = new VaultKeyData(Convert.FromBase64String(sec.value), _area);
                                _keys.Add(key.Id, key);
                            }
                        }
                    }
                    if (secrets.nextLink != null)
                        response = await client.GetAsync(secrets.nextLink);
                    else
                        isAtEnd = true;
                } while (!isAtEnd);
            }
        }

        protected async Task<AuthenticationResult> Authenticate()
        {
            var authContext = new AuthenticationContext(_aadInstance);
            var clientCredential = new ClientCredential(_clientId, _appKey);

            return await authContext.AcquireTokenAsync(resource, clientCredential);

        }

        public List<AdmPwd.PDS.KeyStore.KeyData> GetPublicKeys()
        {
            List<AdmPwd.PDS.KeyStore.KeyData> retVal = new List<KeyStore.KeyData>();

            foreach (var key in _keys)
                retVal.Add(GetPublicKey(key.Key));
            return retVal;
        }

        public List<int> SupportedKeySizes
        {
            get
            {
                return new List<int>(_keySizes);
            }
        }

        public string Decrypt(uint keyID, string EncryptedPwd)
        {
            using (var csp = new RSACryptoServiceProvider(new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore }))
            {
                VaultKeyData privKey = _keys[keyID];

                csp.ImportCspBlob(privKey.Key);
                byte[] decryptedData = null;
                decryptedData = csp.Decrypt(Convert.FromBase64String(EncryptedPwd), true);
                return System.Text.UnicodeEncoding.Unicode.GetString(decryptedData); // decrypted password
            }
        }

        public uint GenerateKeyPair(int KeySize)
        {
            CspParameters CSPParam = new CspParameters();
            CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;

            UInt32 KeyID = 1;
            if (_keys.Keys.Count > 0)
                KeyID = _keys.Keys.Max<UInt32>() + 1;
            using (var csp = new RSACryptoServiceProvider(KeySize, CSPParam))
            {
                var privKey = new VaultKeyData(KeyID, csp.ExportCspBlob(true), _area);

                SecretUpdate.SecretUpdate privSecret = privKey.ToSecretUpdate();

                SaveSecret(privSecret, (Guid.NewGuid().ToString())).Wait();

                _keys.Add(KeyID, privKey);
                return KeyID;
            }
        }

        protected async Task SaveSecret(SecretUpdate.SecretUpdate secret, string secretName)
        {
            AuthenticationResult result = await Authenticate();

            UriBuilder ub = new UriBuilder(_vaultUri);
            ub.Path = "/secrets/" + secretName;
            ub.Query = "api-version=" + _apiVersion;


            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SecretUpdate.SecretUpdate));
            using (var stream = new System.IO.MemoryStream())
            {
                serializer.WriteObject(stream, secret);
                stream.Position = 0;

                using (StreamContent content = new StreamContent(stream))
                using (var client = new HttpClient())
                {
                    content.Headers.Add("Content-type", "application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PutAsync(ub.Uri, content);
                    if (!response.IsSuccessStatusCode)
                        throw new KeyStoreException(response.ReasonPhrase);
                    var data = await response.Content.ReadAsStringAsync();

                }
            }
        }

        public AdmPwd.PDS.KeyStore.KeyData GetPublicKey(uint KeyID)
        {
            if (!_keys.Keys.Contains(KeyID))
                throw new ArgumentException(string.Format("Key with this ID does not exist: {0}", KeyID));
            if (_keys[KeyID] == null)
                return null;
            using (var csp = new RSACryptoServiceProvider(new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore }))
            {
                csp.ImportCspBlob(_keys[KeyID].Key);
                byte[] pubKey = csp.ExportCspBlob(false);
                VaultKeyData pubKeyData = new VaultKeyData(KeyID, pubKey, _keys[KeyID].Area);
                AdmPwd.PDS.KeyStore.KeyData retVal = new KeyStore.KeyData();
                retVal.ID = KeyID;
                retVal.Key = pubKeyData.ToString();
                retVal.Size = csp.KeySize;
                retVal.Type = "CryptoAPI_RSA";
                return retVal;
            }
        }
    }
}
