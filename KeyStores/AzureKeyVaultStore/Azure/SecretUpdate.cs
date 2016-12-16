using System.Collections.Generic;
using System.Runtime.Serialization;


namespace AdmPwd.PDS.AzureKeyStore.SecretUpdate
{

    public class Attributes
    {
        public string enabled { get; set; }
        public int? nbf { get; set; }
        public int? exp { get; set; }
    }
    public class Tags
    {
        public string KeyID { get; set; }
        //public string KeyType { get; set; }
        public string Area { get; set; }
    }

    [DataContract]
    public class SecretUpdate
    {
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public string contentType { get; set; }
        [DataMember]
        public Attributes attributes { get; set; }
        [DataMember]
        public Tags tags { get; set; }

        public SecretUpdate()
        {
            attributes = new Attributes();
            tags = new Tags();
        }
    }
}