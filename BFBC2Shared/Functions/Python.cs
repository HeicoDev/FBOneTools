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
            var process = Process.Start(SharedSettings.PathToPython, "\"" + script + "\"");
            await Task.Run(() => process.WaitForExit());
            process.Close();
        }

        public static async Task ExecuteScript(string script, string target)
        {
            var process = Process.Start(SharedSettings.PathToPython, "\"" + script + "\" \"" + target + "\"");
            await Task.Run(() => process.WaitForExit());
            process.Close();
        }

        public static async Task ExecuteScript(string script, string target, string destination)
        {
            var process = Process.Start(SharedSettings.PathToPython, "\"" + script + "\" \"" + target + "\" \"" + destination + "\"");
            await Task.Run(() => process.WaitForExit());
            process.Close();
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
    }
}
