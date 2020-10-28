using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BFBC2Shared.Data;
using Microsoft.Win32;

namespace BFBC2Shared.Functions
{
    public class Python
    {
        public static async Task ExecuteScript(string script)
        {
            using (var process = Process.Start(SharedSettings.PathToPython, "\"" + script + "\""))
                await Task.Run(() => process.WaitForExit());
        }

        public static async Task ExecuteScript(string script, string target)
        {
            using (var process = Process.Start(SharedSettings.PathToPython, "\"" + script + "\" \"" + target + "\""))
                await Task.Run(() => process.WaitForExit());
        }

        public static async Task ExecuteScript(string script, string target, string destination)
        {
            using (var process = Process.Start(SharedSettings.PathToPython, "\"" + script + "\" \"" + target + "\" \"" + destination + "\""))
                await Task.Run(() => process.WaitForExit());
        }

        public static string ChangePath()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "exe file (.exe)|*.exe";
                ofd.Title = "Select pythonw.exe...";

                if (ofd.ShowDialog() == true)
                {
                    string path = ofd.FileName;

                    if (path.EndsWith("pythonw.exe"))
                    {
                        SharedSettings.PathToPython = path;

                        return path;
                    }
                    else
                    {
                        path = Path.GetDirectoryName(path) + @"\pythonw.exe";

                        if (File.Exists(path))
                        {
                            SharedSettings.PathToPython = path;

                            return path;
                        }
                        else
                        {
                            return String.Empty;
                        }
                    }
                }

                return String.Empty;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());

                return String.Empty;
            }
        }

        public static bool CheckVersion()
        {
            var processInfo = new ProcessStartInfo();
            processInfo.FileName = @"cmd.exe"; 
            processInfo.Arguments = String.Format(@"/c {0}\{1} ", Path.GetDirectoryName(SharedSettings.PathToPython), "python --version");
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = false;
            processInfo.RedirectStandardError = true;
            processInfo.CreateNoWindow = true;

            using (var process = Process.Start(processInfo))
            {
                using (StreamReader reader = process.StandardError)
                {
                    string result = reader.ReadToEnd();

                    if (result.StartsWith("Python 2.7"))
                        return true;
                    else
                        return false;
                }
            }
        }
    }
}
