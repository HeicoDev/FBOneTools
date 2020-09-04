using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Win32;
using BFBC2Toolkit.Data;
using BFBC2Toolkit.Helpers;
using BFBC2Shared.Functions;

namespace BFBC2Toolkit.Functions
{
    public class Mod
    {
        public static async Task<bool> OpenProject(OpenFileDialog ofd)
        {
            try
            {
                await MediaStream.Dispose();

                Dirs.FilesPathMod = ofd.FileName.Replace(@"\ModInfo.ini", "");

                var iniFile = new IniFile(ofd.FileName);

                Dirs.ModName = iniFile.Read("Name", "ModInfo");

                Tree.Populate(UIElements.TreeViewModExplorer, Dirs.FilesPathMod);

                Globals.IsModAvailable = true;

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open mod project! See error.log", "error");

                return true;
            }
        }

        public static async Task<bool> Extract(OpenFileDialog ofd)
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
                                   path = Dirs.Projects + @"\" + name;

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

                                    Log.Write("Extraction aborted.");

                                    return true;
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

                            Tree.Populate(UIElements.TreeViewModExplorer, path);

                            Dirs.FilesPathMod = path;
                            Dirs.ModName = name;

                            Globals.IsModAvailable = true;                            
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to extract mod project! See error.log", "error");

                return true;
            }
        }

        public static async Task<bool> Archive()
        {
            try
            {
                await MediaStream.Dispose();

                string zipFile = Dirs.OutputMods + @"\" + Dirs.ModName + ".zip",
                       projectPath = Dirs.Projects + @"\" + Dirs.ModName;

                if (File.Exists(zipFile))
                    await Task.Run(() => File.Delete(zipFile));

                await Task.Run(() => CleanUp.FilesAndDirs(projectPath));

                await Task.Run(() => ZipFile.CreateFromDirectory(projectPath, zipFile));

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to archive mod project! See error.log", "error");

                return true;
            }
        }
    }
}
