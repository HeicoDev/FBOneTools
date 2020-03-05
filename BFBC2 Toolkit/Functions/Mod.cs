using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Win32;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Mod
    {
        public static async Task OpenProject(OpenFileDialog ofd)
        {
            try
            {
                await MediaStream.Dispose();

                Dirs.filesPathMod = ofd.FileName.Replace(@"\ModInfo.ini", "");

                var iniFile = new IniFile(ofd.FileName);

                Dirs.modName = iniFile.Read("Name", "ModInfo");

                Tree.Populate(Elements.TreeViewModExplorer, Dirs.filesPathMod);

                Vars.isModAvailable = true;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to open mod project! See error.log", "error");
            }
        }

        public static async Task Extract(OpenFileDialog ofd)
        {
            try
            {
                await MediaStream.Dispose();

                using (var fileStream = new FileStream(ofd.FileName, FileMode.Open))
                {
                    using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                    {
                        var entry = await Task.Run(() => zipArchive.GetEntry("ModInfo.ini"));

                        if (entry != null)
                        {
                            string tempFile = Path.GetTempFileName();

                            await Task.Run(() => entry.ExtractToFile(tempFile, true));

                            var iniFile = new IniFile(tempFile);

                            string name = iniFile.Read("Name", "ModInfo"),
                                   path = Dirs.projects + @"\" + name;

                            if (Directory.Exists(path))
                            {
                                var result = MessageBox.Show("A mod with the same name exists already.\nThe old mod must be overwritten to continue.\nDo you want to continue?", "Overwrite existing mod?", MessageBoxButton.YesNo);

                                if (result == MessageBoxResult.Yes)
                                {
                                    await Task.Run(() => Directory.Delete(path, true));

                                    await Task.Run(() => zipArchive.ExtractToDirectory(path));
                                }
                                else
                                {
                                    if (File.Exists(tempFile))
                                        await Task.Run(() => File.Delete(tempFile));

                                    Write.ToEventLog("Extraction aborted.", "");

                                    return;
                                }
                            }
                            else
                            {
                                if (Directory.Exists(path))
                                    await Task.Run(() => Directory.Delete(path, true));

                                await Task.Run(() => zipArchive.ExtractToDirectory(path));
                            }

                            if (File.Exists(tempFile))
                                await Task.Run(() => File.Delete(tempFile));

                            Tree.Populate(Elements.TreeViewModExplorer, path);

                            Dirs.filesPathMod = path;
                            Dirs.modName = name;

                            Vars.isModAvailable = true;                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to extract mod project! See error.log", "error");
            }
        }

        public static async Task Archive()
        {
            try
            {
                await MediaStream.Dispose();

                string zipFile = Dirs.outputMods + @"\" + Dirs.modName + ".zip",
                       projectPath = Dirs.projects + @"\" + Dirs.modName;

                if (File.Exists(zipFile))
                    await Task.Run(() => File.Delete(zipFile));

                Write.ToEventLog("Cleaning up files...", "");

                await Task.Run(() => CleanUp.FilesAndDirs(projectPath));

                await Task.Run(() => ZipFile.CreateFromDirectory(projectPath, zipFile));
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to archive mod project! See error.log", "error");
            }
        }
    }
}
