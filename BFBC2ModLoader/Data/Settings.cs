using System;
using System.IO;
using Microsoft.Win32;
using BFBC2ModLoader.Functions;

namespace BFBC2ModLoader.Data
{
    public class Settings
    {
        public static bool ModsEnabled { get; set; } = true;
        public static bool IsAutoUpdateCheckEnabled { get; set; } = true;

        public static string PathToPython { get; set; } = @"C:\Python27\pythonw.exe";
    }

    public class SettingsHandler
    {
        public static string ChangePythonPath()
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
                        Settings.PathToPython = path;

                        return path;
                    }
                    else
                    {
                        path = Path.GetDirectoryName(path) + @"\pythonw.exe";

                        if (File.Exists(path))
                        {
                            Settings.PathToPython = path;

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
                Write.ToErrorLog(ex);

                return String.Empty;
            }
        }
    }
}
