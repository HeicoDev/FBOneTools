using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BFBC2ModLoader.Data;
using BFBC2ModLoader.Data.Bindings;
using BFBC2Shared.Data;
using BFBC2Shared.Functions;

namespace BFBC2ModLoader.Functions
{
    public class Delete
    {
        public static async Task Mods()
        {
            try
            {
                foreach (var item in UIElements.DataGridModManager.Items)
                {
                    var item1 = item as ModManagerItem;

                    if (item1.ModType.Contains("map"))
                    {
                        string modName = item1.ModName;
                        var mapInfo = new IniFile(Dirs.ModsFolder + "\\" + modName + @"\ModInfo.ini");
                        string mapName = mapInfo.Read("MapID", "ModInfo");

                        if (Directory.Exists(Dirs.LevelsPathPackage + "\\" + mapName))
                            await Task.Run(() => Directory.Delete(Dirs.LevelsPathPackage + "\\" + mapName, true));
                        if (Directory.Exists(Dirs.LevelsPathDist + "\\" + mapName))
                            await Task.Run(() => Directory.Delete(Dirs.LevelsPathDist + "\\" + mapName, true));
                    }
                }

                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                {
                    if (Directory.Exists(kvp.Value))
                        await Task.Run(() => Directory.Delete(kvp.Value, true));
                }

                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbFiles)
                {
                    if (File.Exists(kvp.Value))
                        await Task.Run(() => File.Delete(kvp.Value));
                }

                if (Directory.Exists(Dirs.ModsFolder))
                    await Task.Run(() => Directory.Delete(Dirs.ModsFolder, true));

                if (File.Exists(Dirs.MftRoot))
                    File.Delete(Dirs.MftRoot);
                File.Copy(Dirs.MftOriginal, Dirs.MftRoot);
                if (File.Exists(Dirs.BundleManiRoot))
                    File.Delete(Dirs.BundleManiRoot);
                File.Copy(Dirs.BundleManiOriginal, Dirs.BundleManiRoot);

                UIElements.DataGridModManager.Items.Clear();

                Save.ModsXML();              
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to delete files! See error.log", "error");
            }
        }

        public static async Task TempFiles()
        {
            try
            {
                if (Directory.Exists(Dirs.ExtractPath))
                    await Task.Run(() => Directory.Delete(Dirs.ExtractPath, true));
                if (File.Exists(Dirs.TempText))
                    await Task.Run(() => File.Delete(Dirs.TempText));
                if (File.Exists(Dirs.ModsText))
                    await Task.Run(() => File.Delete(Dirs.ModsText));
                if (File.Exists(Dirs.MapZIP))
                    await Task.Run(() => File.Delete(Dirs.MapZIP));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not remove temp files! See error.log", "error");
            }
        }

        public static async Task Cleanup(string modName, string archivePath)
        {
            try
            {
                if (Directory.Exists(Dirs.ModsFolder + "\\" + modName + @"\_Docs"))
                    await Task.Run(() => Directory.Delete(Dirs.ModsFolder + "\\" + modName + @"\_Docs", true));
                if (Directory.Exists(Dirs.ModsFolder + "\\" + modName + @"\Docs"))
                    await Task.Run(() => Directory.Delete(Dirs.ModsFolder + "\\" + modName + @"\Docs", true));
                if (Directory.Exists(archivePath + @"\_Docs"))
                    await Task.Run(() => Directory.Delete(archivePath + @"\_Docs", true));
                if (Directory.Exists(archivePath + @"\Docs"))
                    await Task.Run(() => Directory.Delete(archivePath + @"\Docs", true));
                if (File.Exists(archivePath + @"\ModInfo.ini"))
                    await Task.Run(() => File.Delete(archivePath + @"\ModInfo.ini"));
                if (File.Exists(Dirs.TempText))
                    await Task.Run(() => File.Delete(Dirs.TempText));
                if (File.Exists(Dirs.MapZIP))
                    await Task.Run(() => File.Delete(Dirs.MapZIP));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to cleanup files! See error.log", "error");
            }
        }
        
        public static void OldVersion()
        {
            try
            {
                if (!File.Exists(Environment.CurrentDirectory + @"\BFBC2 Mod Loader.exe") && !File.Exists(Environment.CurrentDirectory + @"\DotNetZip.dll"))
                    return;

                if (File.Exists(Environment.CurrentDirectory + @"\BFBC2 Mod Loader.exe"))
                    File.Delete(Environment.CurrentDirectory + @"\BFBC2 Mod Loader.exe");
                if (File.Exists(Environment.CurrentDirectory + @"\DotNetZip.dll"))
                    File.Delete(Environment.CurrentDirectory + @"\DotNetZip.dll");
                if (Directory.Exists(Environment.CurrentDirectory + @"\package\levels\mods_common"))
                    Directory.Delete(Environment.CurrentDirectory + @"\package\levels\mods_common", true);
                if (Directory.Exists(Environment.CurrentDirectory + @"\dist\linux\levels\mods_common"))
                    Directory.Delete(Environment.CurrentDirectory + @"\dist\linux\levels\mods_common", true);

                if (File.Exists(Dirs.MftRoot))
                    File.Delete(Dirs.MftRoot);
                File.Copy(Dirs.MftOriginal, Dirs.MftRoot);
                if (File.Exists(Dirs.BundleManiRoot))
                    File.Delete(Dirs.BundleManiRoot);
                File.Copy(Dirs.BundleManiOriginal, Dirs.BundleManiRoot);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to delete old version! See error.log", "error");
            }
        }

        public static void LogFiles()
        {
            try
            {
                if (File.Exists(SharedDirs.ErrorLog))
                    File.Delete(SharedDirs.ErrorLog);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not delete log files! See error.log", "error");
            }
        }
    }
}
