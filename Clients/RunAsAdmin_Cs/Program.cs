using System;
using System.Runtime.InteropServices;
using AdmPwd.PDSUtils;
using AdmPwd.Types;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace RunAsAdmin
{
    class Program
    {
        static LogonFlags dwLogonFlags = LogonFlags.LOGON_WITH_PROFILE;
        static IdentityType accountType = IdentityType.ManagedDomainAccount;

        static int Worker(Params commandParams)
        {
            string domainName = null;
            if (commandParams.User.Contains("\\"))
            {
                string[] pairs = commandParams.User.Split('\\');
                domainName = pairs[0];
                commandParams.User = pairs[1];
                if (domainName == ".")
                    accountType = IdentityType.LocalComputerAdmin;
            }
            if (commandParams.NoLocalProfile)
                dwLogonFlags = LogonFlags.LOGON_NETCREDENTIALS_ONLY;

            try
            {
                PasswordInfo pwdInfo = null;
                switch (accountType)
                {
                    case IdentityType.ManagedDomainAccount:
                        pwdInfo = PdsWrapper.GetPassword(null, commandParams.User, accountType, false, false);
                        break;
                    default:
                        pwdInfo = PdsWrapper.GetPassword(null, System.Environment.GetEnvironmentVariable("COMPUTERNAME"), accountType, false, false);
                        break;
                }

                StartupInfo si = new StartupInfo();
                si.cb = Marshal.SizeOf(si);
                ProcessInformation pi = new ProcessInformation();
                bool rslt = Native.CreateProcessWithLogonW(commandParams.User, domainName, pwdInfo.Password, (uint)dwLogonFlags, null, commandParams.ProgramPath, (uint)(CreationFlags.CREATE_NEW_PROCESS_GROUP | CreationFlags.ABOVE_NORMAL_PRIORITY_CLASS), 0, null, ref si, out pi);
                if (!rslt)
                {
                    throw new System.ComponentModel.Win32Exception(Native.GetLastError());
                }
            }
            catch (PDSException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return -1;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to start process, Win32 return code: {ex.NativeErrorCode.ToString("X2")}");
                return -2;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return -3;
            }

            return 0;
        }

        static int Main(string[] args)
        {
            //model command line options
            var rootCommand = new RootCommand("Sample replacement of Windows RunAs tool, without the need to enter the password of user to run under. " +
                "Password is retrieved automatically behind the scenes via AdmPwd.E SDK")
            {
                new Option(
                    new string[] {"--user","-u" },
                    "User name to use as security context. Allowed formats: domain\\user, user@domain.com or .\\user")
                    {
                        Argument= new Argument()
                        {
                            Arity = new ArgumentArity(1,1)
                        },
                        IsRequired=true
                    },
                new Option(
                    new string[] {"--program-path","-p" },
                    "Path to executable to be started")
                    {
                        Argument= new Argument() 
                        {
                            Arity = new ArgumentArity(1,1)
                        },
                        IsRequired=true
                    },
                new Option(
                    new string[] {"--no-local-profile","-nlp" },
                    "When specified, profile for given user is not created and no user information is stored on machine") 
                    {
                        Argument= new Argument()
                        {
                            Arity = new ArgumentArity(0,1)
                        },
                        IsRequired=false
                    }
            };

            //register worker to execute for command line
            rootCommand.Handler = CommandHandler.Create<Params>(Worker);

            return rootCommand.Invoke(args);
        }
    }
}
