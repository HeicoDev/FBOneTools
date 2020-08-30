using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Helpers;

namespace BFBC2_Toolkit.Functions
{
    public class Fbrb
    {
        public static async Task<bool> Extract(OpenFileDialog ofd)
        {
            try
            {
                await MediaStream.Dispose();

                if (Directory.Exists(Dirs.FilesPathData) && !Globals.IsGameProfile)
                    await Task.Run(() => Directory.Delete(Dirs.FilesPathData, true));

                var process = Process.Start(Settings.PathToPython, "\"" + Dirs.ScriptArchive + "\" \"" + ofd.FileName.Replace(@"\", @"\\") + "\"");
                await Task.Run(() => process.WaitForExit());

                Dirs.FilesPathData = ofd.FileName.Replace(".fbrb", " FbRB");

                Write.ToEventLog("Cleaning up files, please wait...", "");

                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.FilesPathData));

                Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.FilesPathData);

                Globals.IsDataAvailable = true;
                Globals.IsGameProfile = false;

                return false;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to extract fbrb file! See error.log", "error");

                return true;
            }
        }

        public static async Task<bool> Archive()
        {
            try
            {
                await MediaStream.Dispose();

                var process = Process.Start(Settings.PathToPython, "\"" + Dirs.ScriptArchive + "\" \"" + Dirs.FilesPathData.Replace(@"\", @"\\") + "\"");
                await Task.Run(() => process.WaitForExit());

                return false;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to archive fbrb file! See error.log", "error");

                return true;
            }
        }
    }
}
