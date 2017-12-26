using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdmPwd.PDSUtils;
using AdmPwd.Types;

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

            string adminAccountName = null;
            string server = null;
            string domainName = null;


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
                form.SetServerName(server);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(form);
            }
            catch(Exception)
            {
                //do nothing now
            }
        }

        static void Usage()
        {
            MessageBox.Show("Parameters:\n/server:<server to connect> /user:<user@domain>", "Usage", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
