using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdmPwd.Types;
using AdmPwd.PDSUtils;

namespace RDPClient
{
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

                if (arg.StartsWith("/?", StringComparison.CurrentCultureIgnoreCase))
                {
                    Usage();
                    return;
                }
            }
            if (adminAccountName == null || server == null)
            {
                Usage();
                return;
            }

            if (adminAccountName.Contains("\\"))
            {
                string[] pairs = adminAccountName.Split('\\');
                domainName = pairs[0];
                adminAccountName = pairs[1];
            }
            string password = null;
            try
            {
                PasswordInfo pwdInfo = PdsWrapper.GetManagedAccountPassword(null, adminAccountName, false);
                password = pwdInfo.Password;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve password for account {adminAccountName}\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                var form = new Form1();
                form.SetCredentials(adminAccountName, domainName, password);
                form.SetServerName(server, port);

                Application.Run(form);
            }
            catch(Exception)
            {
                //do nothing now
            }
        }

        static void Usage()
        {
            MessageBox.Show("Parameters:\n/server:<server to connect> /user:<user@domain> [/port:<rdp port>]", "Usage", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
