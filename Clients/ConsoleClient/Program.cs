using System;
using AdmPwd.PDSUtils;
using System.ServiceModel;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(string.Format("Getting the password for computer {0}", args[0]));
                AdmPwd.Types.PasswordInfo pwdData = PdsWrapper.GetLocalAdminPassword(ForestName: string.Empty, ComputerName: args[0], IncludePasswordHistory: false, ComputerIsDeleted: false);
                Console.WriteLine(string.Format("Password: {0}", pwdData.Password));
                Console.WriteLine(string.Format("Expires: {0}", pwdData.ExpirationTimestamp.ToString()));

                Console.Write("Resetting password ... ");

                PdsWrapper.ResetLocalAdminPassword(ForestName: string.Empty, ComputerName: args[0], WhenEffective: DateTime.Now);
                Console.WriteLine("done");
            }
            catch (AdmPwd.Types.PDSException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
