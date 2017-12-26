using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using AdmPwd.PDSUtils;
using AdmPwd.Types;

namespace RunAsAdmin
{
    class Program
    {
        static void Main(string[] args)
        {
            string adminAccountName=null;
            string pathToExecutable=null;
            string domainName = null;

            LogonFlags dwLogonFlags = LogonFlags.LOGON_WITH_PROFILE;

            foreach (string arg in args)
            {
                if (arg.StartsWith("/user:", StringComparison.CurrentCultureIgnoreCase))
                {
                    adminAccountName = arg.Substring(6);
                    continue;
                }
                if (arg.StartsWith("/path:", StringComparison.CurrentCultureIgnoreCase))
                {
                    pathToExecutable = arg.Substring(6);
                    continue;
                }
                if (arg.StartsWith("/noLocalProfile", StringComparison.CurrentCultureIgnoreCase))
                {
                    dwLogonFlags = LogonFlags.LOGON_NETCREDENTIALS_ONLY;
                    continue;
                }
                if (arg.StartsWith("/?", StringComparison.CurrentCultureIgnoreCase))
                {
                    Usage();
                    return;
                }
            }
            if(adminAccountName==null || pathToExecutable==null)
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
            try
            {
                PasswordInfo pwdInfo = PdsWrapper.GetManagedAccountPassword(null, adminAccountName, false);

                StartupInfo si = new StartupInfo();
                si.cb = Marshal.SizeOf(si);
                ProcessInformation pi = new ProcessInformation();
                bool rslt = Native.CreateProcessWithLogonW(adminAccountName, domainName, pwdInfo.Password, (uint)dwLogonFlags, null, pathToExecutable, (uint)(CreationFlags.CREATE_NEW_PROCESS_GROUP | CreationFlags.ABOVE_NORMAL_PRIORITY_CLASS), 0, null, ref si, out pi);
                if (!rslt)
                {
                    throw new System.ComponentModel.Win32Exception(Native.GetLastError());
                }
            }
            catch(PDSException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            catch(System.ComponentModel.Win32Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to start process, Win32 return code: {ex.NativeErrorCode.ToString("X2")}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        static void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("RunAsAdmin.exe /path:<path to executable to run> [/user:<name of admin account>] [/noLocalProfile]");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("Runs command prompt as domain account:");
            Console.WriteLine("  RunAsAdmin /path:%SystemRoot%\\system32\\cmd.exe /user:myaccount@mydomain.com");
            Console.WriteLine("");
            Console.WriteLine("Runs command prompt as domain user without creating local profile and without caching the password on local machine:");
            Console.WriteLine("  RunAsAdmin /path:%SystemRoot%\\system32\\cmd.exe /user:mydomain\\myaccount /noLocalProfile");
            Console.WriteLine("");

        }
    }
}
