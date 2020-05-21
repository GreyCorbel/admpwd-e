using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdmPwd.Types;
using AdmPwd.PDSUtils;

namespace RDPClient
{
    public class RdpOptions
    {
        
        public bool RedirectDrives { get; set; }
        public bool RedirectPrinters { get; set; }
        public bool RedirectSmartCards { get; set; }
        public bool RedirectDevices { get; set; }
    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string adminAccountName = null;
            string server = null;
            string domainName = null;
            ushort port = 3389;

            var Options = new RdpOptions();
            foreach (string arg in args)
            {
                if (arg.StartsWith("/user:", StringComparison.CurrentCultureIgnoreCase))
                {
                    adminAccountName = arg.Substring(6);
                    continue;
                }
                if (arg.StartsWith("/server:", StringComparison.CurrentCultureIgnoreCase))
                {
                    server = arg.Substring(8);
                    continue;
                }
                if (arg.StartsWith("/port:", StringComparison.CurrentCultureIgnoreCase))
                {
                    ushort.TryParse(arg.Substring(6), out port);
                    continue;
                }

                if(arg.Equals("/RedirectDrives", StringComparison.CurrentCultureIgnoreCase))
                {
                    Options.RedirectDrives = true;
                    continue;
                }
                if (arg.Equals("/RedirectPrinters", StringComparison.CurrentCultureIgnoreCase))
                {
                    Options.RedirectPrinters = true;
                    continue;
                }
                if (arg.Equals("/RedirectSmartCards", StringComparison.CurrentCultureIgnoreCase))
                {
                    Options.RedirectSmartCards = true;
                    continue;
                }
                if (arg.Equals("/RedirectDevices", StringComparison.CurrentCultureIgnoreCase))
                {
                    Options.RedirectDevices = true;
                    continue;
                }
                if (arg.Equals("/RedirectAll", StringComparison.CurrentCultureIgnoreCase))
                {
                    Options.RedirectDrives = true;
                    Options.RedirectDevices = true;
                    Options.RedirectPrinters = true;
                    Options.RedirectSmartCards = true;
                    continue;
                }
                if (arg.StartsWith("/?", StringComparison.CurrentCultureIgnoreCase))
                {
                    Usage();
                    return;
                }
            }
            if (server == null)
            {
                Usage();
                return;
            }
            PasswordInfo pwdInfo = null;
            try
            {
                //user name to use
                if (adminAccountName.Contains('\\'))
                {
                    //domain\sAMAccountName
                    string[] pairs = adminAccountName.Split('\\');
                    domainName = pairs[0];
                    adminAccountName = pairs[1];
                    pwdInfo = PdsWrapper.GetPassword(null, adminAccountName, IdentityType.ManagedDomainAccount, false, false);
                }
                else if (adminAccountName.Contains('@'))
                {
                    //upn
                    pwdInfo = PdsWrapper.GetPassword(null, adminAccountName, IdentityType.ManagedDomainAccount, false, false);
                }
                else
                {
                    //local account
                    domainName = server;
                    if (adminAccountName == null)
                    {
                        //default admin name
                        adminAccountName = "administrator";
                    }
                    pwdInfo = PdsWrapper.GetPassword(null, server, IdentityType.LocalComputerAdmin, false, false);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve password for account {adminAccountName}\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                var form = new MainForm();

                var cfg = new Configuration();
                Int64 windowSize = cfg.LastWindowSize;
                
                if(windowSize>0)
                {
                    form.SetSize(windowSize);
                }
                form.SetCredentials(adminAccountName, domainName, pwdInfo.Password);
                form.SetServerName(server, port);
                form.SetOptions(Options);

                Application.Run(form);

                cfg.LastWindowSize = form.GetSize();
                cfg.Update();
            }
            catch(Exception)
            {
                //do nothing now
            }
        }

        static void Usage()
        {
            MessageBox.Show("Parameters:\n/server:<server to connect> /user:<user@domain> [/port:<rdp port>] [/redirectDrives] [/redirectPrinters] [/redirectSmartCards] [/redirectDevices] [/redirectAll]", "Usage", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
