using KProcess.Ksmed.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test connection to database");
            Console.WriteLine("ConnectionString:");
            Console.WriteLine(ConnectionStringsSecurity.GetConnectionString(null).ConnectionString);
            try
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    var test = context.Users.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :");
                var currentEx = ex;
                while (currentEx != null)
                {
                    Console.WriteLine(currentEx.Message);
                    currentEx = currentEx.InnerException;
                }
            }
        }
    }
}
