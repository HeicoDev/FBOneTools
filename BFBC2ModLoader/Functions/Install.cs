using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BFBC2ModLoader.Data;
using BFBC2ModLoader.Data.Bindings;
using BFBC2Shared.Functions;

namespace BFBC2ModLoader.Functions
{
    public class Install
    {
        public static async Task Mod(string pathFile, bool checkForConflicts)
        {
            try
            {
                //Cleanup
                if (Directory.Exists(Dirs.ExtractPath))
                    Directory.Delete(Dirs.ExtractPath, true);
                if (File.Exists(Dirs.TempText))
                    File.Delete(Dirs.TempText);

                try
                {
                    using (var fileStream = new FileStream(pathFile, FileMode.Open))
                    {
                        using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                        {
                            foreach (ZipArchiveEntry entry in zipArchive.Entries)
                            {
                                if (entry.Name == String.Empty)
                                    continue;

                                await Task.Run(() => Directory.CreateDirectory(Path.Combine(Dirs.ExtractPath, entry.FullName).Replace(entry.Name, "")));
                                await Task.Run(() => entry.ExtractToFile(Path.Combine(Dirs.ExtractPath, entry.FullName), true));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    Log.Write("Could not extract archive to workspace! See error.log", "error");
                }

                //Check if valid mod/ModInfo.ini exists & directories correct
                if (!File.Exists(Dirs.ExtractPath + @"\ModInfo.ini"))
                {
                    await Delete.TempFiles();

                    Log.Write("Not a valid BFBC2 mod. Aborted installation!", "warning");

                    return;
                }

                bool validDirExists = false;
                
                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                {
                    if (Directory.Exists(Dirs.ExtractPath + @"\" + kvp.Key))
                    {
                        validDirExists = true;

                        break;
                    }
                }

                if (Directory.Exists(Dirs.ExtractPath + @"\Package") || Directory.Exists(Dirs.ExtractPath + @"\Dist"))
                    validDirExists = true;

                if (!validDirExists)
                {
                    await Delete.TempFiles();

                    Log.Write("Not a valid BFBC2 mod. Aborted installation!", "warning");

                    return;
                }

                var modInfo = new IniFile(Dirs.ExtractPath + @"\ModInfo.ini");

                string modName = modInfo.Read("Name", "ModInfo"),
                       modType = modInfo.Read("Type", "ModInfo"),
                       modVersion = modInfo.Read("Version", "ModInfo"),
                       modAuthor = modInfo.Read("Author", "ModInfo"),
                       modMapID = modInfo.Read("MapID", "ModInfo"),
                       modImage = modInfo.Read("Image", "ModInfo"),
                       modLink = modInfo.Read("Link", "ModInfo"),
                       archivePath = Dirs.ModsCommon;

                if (modName == String.Empty || modType == String.Empty)
                {
                    await Delete.TempFiles();

                    Log.Write("Not a valid BFBC2 mod. Aborted installation!", "warning");

                    return;
                }

                //Check type of mod/archive
                if (modType.Contains("map"))
                {
                    archivePath = Dirs.StartupPath;
                }

                //Check for conflicts
                bool b = false;

                if (checkForConflicts)
                {
                    var swA = new StreamWriter(Dirs.TempText, true);
                    var swB = new StreamWriter(Dirs.ModsText, true);
                    swA.Close();
                    swB.Close();

                    foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                    {
                        string[] files = { "" };

                        if (Directory.Exists(Dirs.ExtractPath + @"\" + kvp.Key))
                        {
                            files = Directory.GetFiles(Dirs.ExtractPath + @"\" + kvp.Key, "*.*", System.IO.SearchOption.AllDirectories);
                            File.AppendAllLines(Dirs.TempText, files);
                        }

                        string[] filesB = { "" };

                        if (Directory.Exists(kvp.Value))
                        {
                            filesB = Directory.GetFiles(kvp.Value, "*.*", System.IO.SearchOption.AllDirectories);
                            File.AppendAllLines(Dirs.ModsText, filesB);
                        }
                    }

                    string textTemp = File.ReadAllText(Dirs.TempText),
                           textMods = File.ReadAllText(Dirs.ModsText);

                    foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                    {
                        textTemp = textTemp.Replace(Dirs.ExtractPath + @"\" + kvp.Key, "");
                        textMods = textMods.Replace(kvp.Value, "");
                    }

                    File.WriteAllText(Dirs.TempText, textTemp);
                    File.WriteAllText(Dirs.ModsText, textMods);
                    
                    var fileA = File.ReadAllLines(Dirs.ModsText);
                    var fileB = File.ReadAllLines(Dirs.TempText);

                    //Make the actual check for conflicts
                    var hashSet = new HashSet<string>(fileA);

                    foreach (string str in fileB)
                    {                       
                        if (str == String.Empty)
                            continue;
                        
                        if (hashSet.Contains(str))
                        {
                            b = true;
                            break;
                        }
                        else
                        {
                            b = false;
                        }
                    }
                }

                if (b == true)
                {
                    //Overwrite dialog box
                    Log.Write("Conflict detected! You may try to merge the files manually.", "warning");

                    var result = MessageBox.Show("Conflict detected! Do you want to overwrite the existing files?\nIf you just update a mod go ahead and click 'Yes'.\nOverwriting other mods is not recommended, but you can still try it.\nIf you face any issues, try to change the load order or just delete this mod again. Click 'No' to not install this mod.", "Conflict detected!", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        await InstallWithOverwrite(pathFile, modName, modType, modVersion, modAuthor, modMapID, modImage, modLink, archivePath);
                    }

                    //Clean up if no overwrite/installation
                    else if (result == MessageBoxResult.No)
                    {
                        await Delete.TempFiles();

                        Log.Write("Click 'Install Mod' to select the mod you want to install.");
                    }
                }
                else
                {
                    await InstallWithOverwrite(pathFile, modName, modType, modVersion, modAuthor, modMapID, modImage, modLink, archivePath);
                }

                Misc.OrderNumber();
                Save.ModsXML();
                await Delete.TempFiles();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Ops, something went wrong! See error.log", "error");
            }
        }

        private static async Task InstallWithOverwrite(string pathFile, string modName, string modType, string modVersion, string modAuthor, string modMapID, string modImage, string modLink, string archivePath)
        {
            if (Directory.Exists(Dirs.ModsFolder + "\\" + modName))
                Directory.Delete(Dirs.ModsFolder + "\\" + modName, true);

            //Install with overwrite
            using (var fileStream = new FileStream(pathFile, FileMode.Open))
            {
                using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zipArchive.Entries)
                    {
                        if (entry.Name == String.Empty)
                            continue;

                        await Task.Run(() => Directory.CreateDirectory(Path.Combine(archivePath, entry.FullName).Replace(entry.Name, "")));
                        await Task.Run(() => Directory.CreateDirectory(Path.Combine(Dirs.ModsFolder + "\\" + modName, entry.FullName).Replace(entry.Name, "")));
                        await Task.Run(() => entry.ExtractToFile(Path.Combine(archivePath, entry.FullName), true));
                        await Task.Run(() => entry.ExtractToFile(Path.Combine(Dirs.ModsFolder + "\\" + modName, entry.FullName), true));
                    }
                }
            }

            //Add mod to Data Grid View                    
            bool isModAvailable = false;

            foreach (var item in UIElements.DataGridModManager.Items)
            {
                var itemMM = item as ModManagerItem;

                if (itemMM.ModName == modName)
                {
                    isModAvailable = true;
                    break;
                }
            }

            if (!isModAvailable)
            {
                UIElements.DataGridModManager.Items.Add(new ModManagerItem() { ModOrder = "", ModEnabled = true, ModName = modName, ModVersion = modVersion, ModAuthor = modAuthor, ModType = modType, ModMapID = modMapID, ModImage = modImage, ModLink = modLink });
            }
            else
            {
                int rowIndex = -1;
                foreach (var item in UIElements.DataGridModManager.Items)
                {
                    var item1 = item as ModManagerItem;

                    if (item1.ModName.Equals(modName))
                    {
                        rowIndex = UIElements.DataGridModManager.Items.IndexOf(item);
                        var item2 = UIElements.DataGridModManager.Items[rowIndex] as ModManagerItem;
                        item2.ModEnabled = true;
                        item2.ModVersion = modVersion;
                        break;
                    }
                }
            }

            if (File.Exists(Dirs.MftRoot))
                File.Delete(Dirs.MftRoot);
            File.Copy(Dirs.MftModded, Dirs.MftRoot);
            if (File.Exists(Dirs.BundleManiRoot))
                File.Delete(Dirs.BundleManiRoot);
            File.Copy(Dirs.BundleManiModded, Dirs.BundleManiRoot);

            await Delete.Cleanup(modName, archivePath);

            //Create archive or not depending on the info in ModInfo.ini
            if (modType.Contains("map"))
            {
                await Delete.TempFiles();
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                {
                    if (Directory.Exists(Dirs.ExtractPath + @"\" + kvp.Key))
                    {
                        await Python.ExecuteScript(Dirs.ScriptFbrb, kvp.Value);
                    }
                }

                if (Directory.Exists(Dirs.ExtractPath))
                    Directory.Delete(Dirs.ExtractPath, true);
                if (File.Exists(Dirs.ModsText))
                    File.Delete(Dirs.ModsText);
            }
        }

        public static async Task Mods()
        {
            try
            {
                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                {
                    if (Directory.Exists(kvp.Value))
                        Directory.Delete(kvp.Value, true);
                }

                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbFiles)
                {
                    if (File.Exists(kvp.Value))
                        File.Delete(kvp.Value);
                }

                if (File.Exists(Dirs.MftRoot))
                    File.Delete(Dirs.MftRoot);
                File.Copy(Dirs.MftModded, Dirs.MftRoot);
                if (File.Exists(Dirs.BundleManiRoot))
                    File.Delete(Dirs.BundleManiRoot);
                File.Copy(Dirs.BundleManiModded, Dirs.BundleManiRoot);

                foreach (var item in UIElements.DataGridModManager.Items)
                {
                    var itemMM = item as ModManagerItem;

                    string modName = itemMM.ModName,
                           modType = itemMM.ModType,
                           modEnabled = itemMM.ModEnabled.ToString(),
                           modFolder = Dirs.ModsFolder + "\\" + modName;

                    var mapInfo = new IniFile(Dirs.ModsFolder + "\\" + modName + @"\ModInfo.ini");

                    string mapName = mapInfo.Read("MapID", "ModInfo");

                    if (modEnabled.Contains("False") && modType.Contains("map"))
                    {
                        if (Directory.Exists(Dirs.LevelsPathPackage + "\\" + mapName))
                            Directory.Delete(Dirs.LevelsPathPackage + "\\" + mapName, true);
                        if (Directory.Exists(Dirs.LevelsPathDist + "\\" + mapName))
                            Directory.Delete(Dirs.LevelsPathDist + "\\" + mapName, true);
                    }

                    if (modEnabled.Contains("True"))
                    {                        
                        if (modType.Contains("map"))
                        {
                            if (Directory.Exists(modFolder))
                            {
                                foreach (string dirPath in Directory.GetDirectories(modFolder, "*", System.IO.SearchOption.AllDirectories))
                                    await Task.Run(() => Directory.CreateDirectory(dirPath.Replace(modFolder, Dirs.StartupPath)));

                                foreach (string newPath in Directory.GetFiles(modFolder, "*.*", System.IO.SearchOption.AllDirectories))
                                    await Task.Run(() => File.Copy(newPath, newPath.Replace(modFolder, Dirs.StartupPath), true));

                                if (File.Exists(Dirs.StartupPath + @"\ModInfo.ini"))
                                    File.Delete(Dirs.StartupPath + @"\ModInfo.ini");
                            }
                        }
                        else 
                        {
                            if (Directory.Exists(modFolder))
                            {
                                foreach (string dirPath in Directory.GetDirectories(modFolder, "*", System.IO.SearchOption.AllDirectories))
                                    await Task.Run(() => Directory.CreateDirectory(dirPath.Replace(modFolder, Dirs.ModsCommon)));

                                foreach (string newPath in Directory.GetFiles(modFolder, "*.*", System.IO.SearchOption.AllDirectories))
                                    await Task.Run(() => File.Copy(newPath, newPath.Replace(modFolder, Dirs.ModsCommon), true));

                                if (File.Exists(Dirs.ModsCommon + @"\ModInfo.ini"))
                                    File.Delete(Dirs.ModsCommon + @"\ModInfo.ini");
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, string> kvp in Dirs.FbrbDirs)
                {
                    if (Directory.Exists(kvp.Value))
                    {
                        if (Directory.EnumerateFileSystemEntries(kvp.Value).Any())
                        {
                            await Python.ExecuteScript(Dirs.ScriptFbrb, kvp.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not (re)install mods! See error.log", "error");
            }
        }
    }
}
