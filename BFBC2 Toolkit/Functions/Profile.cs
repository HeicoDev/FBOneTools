using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using BFBC2_Toolkit.Data;
using System.Windows;
using System.Xml.Linq;
using System.Xml;

namespace BFBC2_Toolkit.Functions
{
    public class Profile
    {
        public static async Task Add(OpenFileDialog ofd)
        {
            try
            {
                if (ofd.SafeFileName != "BFBC2Game.exe" && ofd.SafeFileName != "Frost.Game.Main_Win32_Final.exe")
                {
                    Write.ToEventLog("Not a valid game or server executable!", "warning");
                    return;
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

                if (Directory.Exists(Dirs.games + @"\" + gameId))
                {
                    var result = MessageBox.Show("There is already a profile for this game!\nDo you want to overwrite it?", "Warning!", MessageBoxButton.YesNo);

                    if (result != MessageBoxResult.Yes)
                        return;

                    Directory.Delete(Dirs.games + @"\" + gameId, true);
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
                    foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                    {
                        if (file.EndsWith(kvp.Key))
                        {
                            filesCountB++;

                            Write.ToEventLog("Extracting fbrb archive " + filesCountB + " of " + filesCountA + "...", "");

                            var process = Process.Start(Dirs.scriptArchive, "\"" + file.Replace(@"\", @"\\"));
                            await Task.Run(() => process.WaitForExit());

                            await Task.Run(() => Directory.CreateDirectory(Dirs.games + @"\" + gameId + @"\" + kvp.Value));

                            foreach (string dir in Directory.GetDirectories(file.Replace(kvp.Key, kvp.Value), "*", SearchOption.AllDirectories))
                                await Task.Run(() => Directory.CreateDirectory(dir.Replace(file.Replace(kvp.Key, kvp.Value), Dirs.games + @"\" + gameId + @"\" + kvp.Value)));

                            foreach (string filePath in Directory.GetFiles(file.Replace(kvp.Key, kvp.Value), "*.*", SearchOption.AllDirectories))
                                await Task.Run(() => File.Copy(filePath, filePath.Replace(file.Replace(kvp.Key, kvp.Value), Dirs.games + @"\" + gameId + @"\" + kvp.Value), true));

                            var xmlDoc = new XmlDocument();
                            xmlDoc.Load(Dirs.configGames);
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

                            xmlDoc.Save(Dirs.configGames);

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to create game profile! See error.log", "error");
            }
        }

        private static int CountFbrbArchives(IEnumerable<string> files)
        {
            int filesCountA = 0;

            foreach (string file in files)
            {
                foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
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
