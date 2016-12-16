using System;

namespace AdmPwd.PDS.AzureKeyStore
{

    public enum KeyType
    {
        Public,
        Private
    }
    public class KeyData
    {
        public UInt32 Id;
        public byte[] key;
        public string area;

        public byte[] rawData()
        {
            byte[] idBytes = BitConverter.GetBytes(Id);
            byte[] rv = new byte[idBytes.Length + key.Length];
            System.Buffer.BlockCopy(idBytes, 0, rv, 0, idBytes.Length);
            System.Buffer.BlockCopy(key, 0, rv, idBytes.Length, key.Length);
            return rv;
        }

        public KeyData(UInt32 id, byte[] key, string area)
        {
            Id = id;
            this.key = key;
            this.area = area;
        }

        public KeyData(byte[] data, string area)
        {
            this.area = area;
            // get Id in leading 4 bytes
            byte[] idBytes = new byte[4];
            System.Buffer.BlockCopy(data, 0, idBytes, 0, 4);
            this.Id = BitConverter.ToUInt32(idBytes, 0);

            // get key data from the rest of byte array
            this.key = new byte[data.Length - idBytes.Length];
            System.Buffer.BlockCopy(data, idBytes.Length, this.key, 0, data.Length - idBytes.Length);
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

            sec.tags.Area = this.area;
            sec.tags.KeyID = this.Id.ToString();

            return sec;
        }
    }
}