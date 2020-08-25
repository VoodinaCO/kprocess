using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KProcess.KSmed.DbDeployer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = 3000; // x10
            Parameters parameters = new Parameters();

            int result = ParseArgs.Parse(args, parameters);
            if (result == 1)
                ExecuteSqlCmd(parameters);
            else
                ParseArgs.ShowHelp();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ExecuteSqlCmd(Parameters parameters)
        {
            StringBuilder command = new StringBuilder();

            //command.Append("SQLCMD ");
            command.Append("-v "); // Prepare for variables
            command.Append(String.Format("DefaultDataPath={0} ", parameters.DataPath)); // Default DataPath
            command.Append(String.Format("DefaultLogPath={0} ", parameters.LogPath)); // Default DataPath
            command.Append(String.Format("DatabaseName={0} ", parameters.DatabaseName)); // Database Name

            foreach (var item in parameters.CustomsParameters)
                command.Append(String.Format("{0}={1} ", item.Key, item.Value));

            if (!String.IsNullOrEmpty(parameters.UserLogin))
                command.Append(String.Format("-U {0} ", parameters.UserLogin));
            if (!String.IsNullOrEmpty(parameters.UserPassword))
                command.Append(String.Format("-P {0} ", parameters.UserPassword));
            if (!String.IsNullOrEmpty(parameters.ServerName))
                command.Append(String.Format("-S {0} ", parameters.ServerName));
            
            command.Append(String.Format("-i {0}", parameters.SqlFilePath)); // SQL script file path

            ProcessStartInfo psi = new ProcessStartInfo("SQLCMD");
            psi.Arguments = command.ToString();
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            Console.WriteLine("Command transcription...");
            Console.WriteLine("SQLCMD {0}", command);
            Console.WriteLine();

            using (Process sqlcmdProcess = Process.Start(psi))
            {
                //sqlcmdProcess.WaitForExit(5000);

                while (!sqlcmdProcess.HasExited)
                {
                    string line = sqlcmdProcess.StandardOutput.ReadLine();
                    while (line != null)
                    {
                        Console.WriteLine(line);
                        line = sqlcmdProcess.StandardOutput.ReadLine();
                    }
                    Console.WriteLine();
                    System.Threading.Thread.Sleep(100);
                }

                Console.WriteLine(sqlcmdProcess.StandardError.ReadToEnd());
                Console.WriteLine();
                Console.WriteLine("Command executed. Return code: {0} at {1}", sqlcmdProcess.ExitCode, sqlcmdProcess.ExitTime);
            }
        }
    }
}
