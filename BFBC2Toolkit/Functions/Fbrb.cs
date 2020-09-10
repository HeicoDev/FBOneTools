using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using BFBC2Toolkit.Data;
using BFBC2Toolkit.Helpers;
using BFBC2Shared.Functions;

namespace BFBC2Toolkit.Functions
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

                await Python.ExecuteScript(Dirs.ScriptFbrb, ofd.FileName);

                Dirs.FilesPathData = ofd.FileName.Replace(".fbrb", " FbRB");

                Log.Write("Cleaning up files, please wait...");

                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.FilesPathData));

                Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.FilesPathData);

                Globals.IsDataAvailable = true;
                Globals.IsGameProfile = false;

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to extract fbrb file! See error.log", "error");

                return true;
            }
        }

        public static async Task<bool> Archive()
        {
            try
            {
                await MediaStream.Dispose();

                await Python.ExecuteScript(Dirs.ScriptFbrb, Dirs.FilesPathData);

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to archive fbrb file! See error.log", "error");

                return true;
            }
        }
    }
}
