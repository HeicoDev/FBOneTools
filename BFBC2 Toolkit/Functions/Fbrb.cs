using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Fbrb
    {
        public static async Task Extract(OpenFileDialog ofd)
        {
            try
            {
                await MediaStream.Dispose();

                if (Directory.Exists(Dirs.filesPathData) && Vars.isGameProfile == false)
                    await Task.Run(() => Directory.Delete(Dirs.filesPathData, true));

                var process = Process.Start(Dirs.scriptArchive, "\"" + ofd.FileName.Replace(@"\", @"\\"));
                await Task.Run(() => process.WaitForExit());

                Dirs.filesPathData = ofd.FileName.Replace(".fbrb", " FbRB");

                Write.ToEventLog("Cleaning up files, please wait...", "");

                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.filesPathData));

                Tree.Populate(Elements.TreeViewDataExplorer, Dirs.filesPathData);

                Vars.isGameProfile = false;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to extract fbrb file! See error.log", "error");
            }
        }

        public static async Task Archive()
        {
            try
            {
                await MediaStream.Dispose();

                var process = Process.Start(Dirs.scriptArchive, "\"" + Dirs.filesPathData.Replace(@"\", @"\\"));
                await Task.Run(() => process.WaitForExit());
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to archive fbrb file! See error.log", "error");
            }
        }
    }
}
