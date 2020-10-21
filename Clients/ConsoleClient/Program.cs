using System;
using AdmPwd.PDSUtils;
using System.ServiceModel;
using AdmPwd.Types;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    Console.WriteLine(string.Format("Getting the password for computer {0}", args[0]));
                    AdmPwd.Types.PasswordInfo pwdData = PdsWrapper.GetPassword(ForestName: string.Empty, Identity: args[0], Type: IdentityType.LocalComputerAdmin, IncludePasswordHistory: false, IsDeleted: false);
                    Console.WriteLine(string.Format("Password: {0}", pwdData.Password));
                    Console.WriteLine(string.Format("Expires: {0}", pwdData.ExpirationTimestamp.ToString()));

                    Console.Write("Resetting password ... ");

                    //request immediate password reset
                    PdsWrapper.ResetPassword(ForestName: string.Empty, Identity: args[0], Type: IdentityType.LocalComputerAdmin, WhenEffective:DateTime.MinValue);
                    Console.WriteLine("done");
                }
                else
                    Console.WriteLine("ERROR: You must pass computer name as parameter");
            }
            catch (PDSException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
