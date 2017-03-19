using System;

namespace AdmPwd.PDS.KeyStore.AzureKeyVault
{
    public class VaultKeyData
    {
        public UInt32 Id;
        public byte[] Key;
        public string Area;

        public byte[] rawData()
        {
            byte[] idBytes = BitConverter.GetBytes(Id);
            byte[] rv = new byte[idBytes.Length + Key.Length];
            System.Buffer.BlockCopy(idBytes, 0, rv, 0, idBytes.Length);
            System.Buffer.BlockCopy(Key, 0, rv, idBytes.Length, Key.Length);
            return rv;
        }

        public VaultKeyData(UInt32 id, byte[] key, string area)
        {
            Id = id;
            Key = key;
            Area = area;
        }

        public VaultKeyData(byte[] data, string area)
        {
            Area = area;
            // get Id in leading 4 bytes
            byte[] idBytes = new byte[4];
            System.Buffer.BlockCopy(data, 0, idBytes, 0, 4);
            this.Id = BitConverter.ToUInt32(idBytes, 0);

            // get key data from the rest of byte array
            this.Key = new byte[data.Length - idBytes.Length];
            System.Buffer.BlockCopy(data, idBytes.Length, this.Key, 0, data.Length - idBytes.Length);
        }

        public override string ToString()
        {
            return Convert.ToBase64String(rawData());
        }

        public SecretUpdate.SecretUpdate ToSecretUpdate()
        {
            SecretUpdate.SecretUpdate sec = new SecretUpdate.SecretUpdate();
            sec.value = this.ToString();
            sec.attributes.enabled = "true";
            sec.attributes.exp = null;
            sec.attributes.nbf = null;

            sec.contentType = null;

            sec.tags.Area = this.Area;
            sec.tags.KeyID = this.Id.ToString();

            return sec;
        }
    }
}