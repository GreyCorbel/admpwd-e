using System.Collections.Generic;
using System.Runtime.Serialization;


namespace AdmPwd.PDS.AzureKeyStore.SecretList
{
    public class Attributes
    {
        public int created { get; set; }
        public int updated { get; set; }
        public bool? enabled { get; set; }
    }

    public class Tags
    {
        public string KeyID { get; set; }
        public string Area { get; set; }
        public string KeyType { get; set; }
    }

    public class SecretInfo
    {
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Tags tags { get; set; }

    }

    [DataContract]
    public class SecretList
    {
        [DataMember]
        public List<SecretInfo> value { get; set; }
        [DataMember]
        public string nextLink { get; set; }
    }
}