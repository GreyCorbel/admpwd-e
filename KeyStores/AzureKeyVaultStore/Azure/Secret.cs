using System.Collections.Generic;
using System.Runtime.Serialization;


namespace AdmPwd.PDS.AzureKeyStore.Secret
{
    public class Attributes
    {
        public bool enabled { get; set; }
        public int created { get; set; }
        public int updated { get; set; }
    }
    public class Tags
    {
        public string KeyID { get; set; }
        public string KeyType { get; set; }
        public string Area { get; set; }
    }
    [DataContract]
    public class Secret
    {
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public Attributes attributes { get; set; }
        [DataMember]
        public Tags tags { get; set; }
    }

}