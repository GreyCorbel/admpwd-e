using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RDPClient
{
    public class Configuration
    {
        string regPath = "Software\\GreyCorbel\\AdmPwd\\RDPClient";

        public long LastWindowSize { get; set; }

        public Configuration()
        {
            var regKey = Registry.CurrentUser.CreateSubKey(regPath, RegistryKeyPermissionCheck.Default);
            object data = regKey.GetValue("LastWindowSize", (Int64)0);
            LastWindowSize = (Int64)data;
            regKey.Close();
        }

        public void Update()
        {
            var regKey = Registry.CurrentUser.CreateSubKey(regPath, RegistryKeyPermissionCheck.Default);
            regKey.SetValue("LastWindowSize", LastWindowSize, RegistryValueKind.QWord);
            regKey.Close();
        }

    }

    
}
