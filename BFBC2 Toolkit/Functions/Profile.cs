using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using BFBC2_Toolkit.Data;
using System.Windows;
using System.Xml.Linq;

namespace BFBC2_Toolkit.Functions
{
    public class Profile
    {
        private static int filesCountA = 0,
                           filesCountB = 0;

        public static async Task Add(OpenFileDialog ofd)
        {
            try
            {
                if (ofd.SafeFileName != "BFBC2Game.exe" && ofd.SafeFileName != "Frost.Game.Main_Win32_Final.exe")
                {
                    Write.ToEventLog("Not a valid game or server executable!", "warning");
                    return;
                }

                string game = "";

                if (ofd.SafeFileName == "BFBC2Game.exe")
                    game = "BFBC2-PC";
                else if (ofd.SafeFileName == "Frost.Game.Main_Win32_Final.exe")
                    game = "BFBC2-Server-PC";

                if (Directory.Exists(Dirs.games + @"\" + game))
                {
                    var result = MessageBox.Show("There is already a profile for this game!\nDo you want to overwrite it?", "Warning!", MessageBoxButton.YesNo);

                    if (result != MessageBoxResult.Yes)
                        return;
                }

                string path = ofd.FileName.Replace(ofd.SafeFileName, "");

                Write.ToEventLog("Searching for fbrb archives...", "");

                var files = await Task.Run(() => Directory.EnumerateFiles(path, "*.fbrb", SearchOption.AllDirectories));

                filesCountA = 0;
                filesCountB = 0;

                await Task.Run(() => CountFbrbArchives(files));

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

                            await Task.Run(() => Directory.CreateDirectory(Dirs.games + @"\" + game + @"\" + kvp.Value));

                            foreach (string dir in Directory.GetDirectories(file.Replace(kvp.Key, kvp.Value), "*", System.IO.SearchOption.AllDirectories))
                                await Task.Run(() => Directory.CreateDirectory(dir.Replace(file.Replace(kvp.Key, kvp.Value), Dirs.games + @"\" + game + @"\" + kvp.Value)));

                            foreach (string filePath in Directory.GetFiles(file.Replace(kvp.Key, kvp.Value), "*.*", System.IO.SearchOption.AllDirectories))
                                await Task.Run(() => File.Copy(filePath, filePath.Replace(file.Replace(kvp.Key, kvp.Value), Dirs.games + @"\" + game + @"\" + kvp.Value), true));

                            /*
                            var xmlDoc = XDocument.Load(Dirs.configGames);
                            var eGames = xmlDoc.Element("Games");
                            var eGame = new XElement("Game");

                            if (game == "BFBC2-PC")
                            {
                                eGame.Add(new XAttribute("Name", "Battlefield Bad Company 2"));
                                eGame.Add(new XAttribute("Platform", "PC"));
                            }
                            if (game == "BFBC2-Server-PC")
                            {
                                eGame.Add(new XAttribute("Name", "Battlefield Bad Company 2 Server"));
                                eGame.Add(new XAttribute("Platform", "PC"));
                            }

                            eGames.Add(game);
                            xmlDoc.Save(Dirs.configGames);
                            */

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

        private static void CountFbrbArchives(IEnumerable<string> files)
        {
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
        }
    }
}
