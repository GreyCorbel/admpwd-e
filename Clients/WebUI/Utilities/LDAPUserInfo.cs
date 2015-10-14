using System;

namespace AdmPwd.Portal.Utilities
{
    [Serializable]
    public class LDAPUserInfo
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Domain { get; set; }
        public string Email { get; set; }
        public string CountryOU { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
    }
}