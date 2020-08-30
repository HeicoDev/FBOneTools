using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Profile
    {        
        public static async Task<bool> Add(OpenFileDialog ofd)
        {
            try
            {
                if (ofd.SafeFileName != "BFBC2Game.exe" && ofd.SafeFileName != "Frost.Game.Main_Win32_Final.exe")
                {
                    Write.ToEventLog("Not a valid game or server executable!", "warning");
                    return true;
                }

                string gameId = "",
                       gameName = "",
                       gamePlatform = "";

                if (ofd.SafeFileName == "BFBC2Game.exe")
                {
                    gameId = "BFBC2-PC";
                    gameName = "Battlefield Bad Company 2";
                    gamePlatform = "PC";
                }
                else if (ofd.SafeFileName == "Frost.Game.Main_Win32_Final.exe")
                {
                    gameId = "BFBC2-Server-PC";
                    gameName = "Battlefield Bad Company 2 Server";
                    gamePlatform = "PC";
                }

                if (Directory.Exists(Dirs.Games + @"\" + gameId))
                {
                    var result = MessageBox.Show("There is already a profile for this game!\nDo you want to overwrite it?", "Warning!", MessageBoxButton.YesNo);

                    if (result != MessageBoxResult.Yes)
                        return true;

                    await Task.Run(() => Directory.Delete(Dirs.Games + @"\" + gameId, true));
                }               

                string path = ofd.FileName.Replace(ofd.SafeFileName, "");

                Write.ToEventLog("Searching for fbrb archives...", "");

                var files = await Task.Run(() => Directory.EnumerateFiles(path, "*.fbrb", SearchOption.AllDirectories));

                int filesCountA = await Task.Run(() => CountFbrbArchives(files)),
                    filesCountB = 0;                

                Write.ToEventLog("Found " + filesCountA + " fbrb archives.", "");
                Write.ToEventLog("This will take a while. Do NOT close the application!", "");

                foreach (string file in files)
                {
                    foreach (KeyValuePair<string, string> kvp in Globals.FbrbFiles)
                    {
                        if (file.EndsWith(kvp.Key))
                        {
                            filesCountB++;

                            Write.ToEventLog("Extracting fbrb archive " + filesCountB + " of " + filesCountA + "...", "");

                            var process = Process.Start(Settings.PathToPython, "\"" + Dirs.ScriptArchive + "\" \"" + file.Replace(@"\", @"\\") + "\"");
                            await Task.Run(() => process.WaitForExit());

                            await Task.Run(() => Directory.CreateDirectory(Dirs.Games + @"\" + gameId + @"\" + kvp.Value));

                            foreach (string dir in Directory.GetDirectories(file.Replace(kvp.Key, kvp.Value), "*", SearchOption.AllDirectories))
                                await Task.Run(() => Directory.CreateDirectory(dir.Replace(file.Replace(kvp.Key, kvp.Value), Dirs.Games + @"\" + gameId + @"\" + kvp.Value)));

                            foreach (string filePath in Directory.GetFiles(file.Replace(kvp.Key, kvp.Value), "*.*", SearchOption.AllDirectories))
                                await Task.Run(() => File.Copy(filePath, filePath.Replace(file.Replace(kvp.Key, kvp.Value), Dirs.Games + @"\" + gameId + @"\" + kvp.Value), true));                           

                            break;
                        }
                    }
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(Dirs.ConfigGames);
                var nodeList = xmlDoc.SelectNodes("/Games/Game");

                bool profileEntryExists = false;

                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].Attributes["Name"].Value == gameName && nodeList[i].Attributes["Platform"].Value == gamePlatform)
                    {
                        profileEntryExists = true;
                        break;
                    }
                }

                if (!profileEntryExists)
                {
                    var rootNode = xmlDoc.DocumentElement;
                    var newElement = xmlDoc.CreateElement("Game");

                    var attrName = xmlDoc.CreateAttribute("Name");
                    attrName.Value = gameName;

                    newElement.Attributes.Append(attrName);

                    var attrPlat = xmlDoc.CreateAttribute("Platform");
                    attrPlat.Value = gamePlatform;

                    newElement.Attributes.Append(attrPlat);

                    rootNode.AppendChild(newElement);
                    xmlDoc.AppendChild(rootNode);
                }

                xmlDoc.Save(Dirs.ConfigGames);

                return false;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to create game profile! See error.log", "error");

                return true;
            }
        }

        public static async Task<bool> Load(GameProfile profile)
        {
            try
            {
                if (Directory.Exists(Dirs.FilesPathData) && Globals.IsGameProfile == false)
                    await Task.Run(() => Directory.Delete(Dirs.FilesPathData, true));

                if (profile.Name == "Battlefield Bad Company 2" && profile.Platform == "PC")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BFBC2-PC");
                    Dirs.FilesPathData = Dirs.Games + @"\BFBC2-PC";
                }
                else if (profile.Name == "Battlefield Bad Company 2 Server" && profile.Platform == "PC")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BFBC2-Server-PC");
                    Dirs.FilesPathData = Dirs.Games + @"\BFBC2-Server-PC";
                }
                else if (profile.Name == "Battlefield Bad Company 2" && profile.Platform == "PS3")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BFBC2-PS3");
                    Dirs.FilesPathData = Dirs.Games + @"\BFBC2-PS3";
                }
                else if (profile.Name == "Battlefield Bad Company 2" && profile.Platform == "Xbox")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BFBC2-Xbox");
                    Dirs.FilesPathData = Dirs.Games + @"\BFBC2-Xbox";
                }
                else if (profile.Name == "Battlefield Bad Company" && profile.Platform == "PS3")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BFBC-PS3");
                    Dirs.FilesPathData = Dirs.Games + @"\BFBC-PS3";
                }
                else if (profile.Name == "Battlefield Bad Company" && profile.Platform == "Xbox")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BFBC-Xbox");
                    Dirs.FilesPathData = Dirs.Games + @"\BFBC-Xbox";
                }
                else if (profile.Name == "Battlefield 1943" && profile.Platform == "PS3")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BF1943-PS3");
                    Dirs.FilesPathData = Dirs.Games + @"\BF1943-PS3";
                }
                else if (profile.Name == "Battlefield 1943" && profile.Platform == "Xbox")
                {
                    Tree.Populate(UIElements.TreeViewDataExplorer, Dirs.Games + @"\BF1943-Xbox");
                    Dirs.FilesPathData = Dirs.Games + @"\BF1943-Xbox";
                }

                return false;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to create game profile! See error.log", "error");

                return true;
            }
        }

        public static async Task<bool> Delete(GameProfile profile)
        {
            try
            {
                if (profile.Name == "Battlefield Bad Company 2" && profile.Platform == "PC")
                {
                    if (Directory.Exists(Dirs.Games + @"\BFBC2-PC"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BFBC2-PC", true));
                }
                else if (profile.Name == "Battlefield Bad Company 2 Server" && profile.Platform == "PC")
                {
                    if (Directory.Exists(Dirs.Games + @"\BFBC2-Server-PC"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BFBC2-Server-PC", true));
                }
                else if (profile.Name == "Battlefield Bad Company 2" && profile.Platform == "PS3")
                {
                    if (Directory.Exists(Dirs.Games + @"\BFBC2-PS3"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BFBC2-PS3", true));
                }
                else if (profile.Name == "Battlefield Bad Company 2" && profile.Platform == "Xbox")
                {
                    if (Directory.Exists(Dirs.Games + @"\BFBC2-Xbox"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BFBC2-Xbox", true));
                }
                else if (profile.Name == "Battlefield Bad Company" && profile.Platform == "PS3")
                {
                    if (Directory.Exists(Dirs.Games + @"\BFBC-PS3"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BFBC-PS3", true));
                }
                else if (profile.Name == "Battlefield Bad Company" && profile.Platform == "Xbox")
                {
                    if (Directory.Exists(Dirs.Games + @"\BFBC-Xbox"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BFBC-Xbox", true));
                }
                else if (profile.Name == "Battlefield 1943" && profile.Platform == "PS3")
                {
                    if (Directory.Exists(Dirs.Games + @"\BF1943-PS3"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BF1943-PS3", true));
                }
                else if (profile.Name == "Battlefield 1943" && profile.Platform == "Xbox")
                {
                    if (Directory.Exists(Dirs.Games + @"\BF1943-Xbox"))
                        await Task.Run(() => Directory.Delete(Dirs.Games + @"\BF1943-Xbox", true));
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(Dirs.ConfigGames);
                var nodeList = xmlDoc.SelectNodes("/Games/Game");

                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].Attributes["Name"].Value == profile.Name && nodeList[i].Attributes["Platform"].Value == profile.Platform)
                    {
                        nodeList[i].ParentNode.RemoveChild(nodeList[i]);
                        break;
                    }
                }

                xmlDoc.Save(Dirs.ConfigGames);

                return false;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to delete game profile! See error.log", "error");

                return true;
            }
        }

        private static int CountFbrbArchives(IEnumerable<string> files)
        {
            int filesCountA = 0;

            foreach (string file in files)
            {
                foreach (KeyValuePair<string, string> kvp in Globals.FbrbFiles)
                {
                    if (file.EndsWith(kvp.Key))
                    {
                        filesCountA++;
                        break;
                    }
                }
            }

            return filesCountA;
        }
    }
}
