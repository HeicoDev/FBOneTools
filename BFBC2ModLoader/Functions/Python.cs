using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using BFBC2ModLoader.Data;

namespace BFBC2ModLoader.Functions
{
    public class Python
    {
        public static async Task Archive()
        {
            try
            {
                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                {
                    if (Directory.Exists(Dirs.ExtractPath + @"\" + kvp.Key))
                    {
                        var process = Process.Start(Settings.PathToPython, "\"" + Dirs.ScriptArchive + "\" \"" + kvp.Value + "\"");
                        await Task.Run(() => process.WaitForExit());
                    }
                }

                if (Directory.Exists(Dirs.ExtractPath))
                    Directory.Delete(Dirs.ExtractPath, true);
                if (File.Exists(Dirs.ModsText))
                    File.Delete(Dirs.ModsText);
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Could not create fbrb archives! See error.log", "error");
            }
        }
    }
}
