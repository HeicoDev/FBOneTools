using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Xml;
using BFBC2Shared.Data;
using BFBC2Shared.Functions;
using BFBC2Toolkit.Data;

namespace BFBC2Toolkit.Functions
{
    public class Check
    {
        public static void Update()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2Toolkit/BFBC2Toolkit/Config/update.config", Dirs.Config + @"\tempUpdate.config");

                    if (File.Exists(Dirs.ConfigUpdate))
                        File.Delete(Dirs.ConfigUpdate);
                    if (File.Exists(Dirs.Config + @"\tempUpdate.config"))
                        File.Move(Dirs.Config + @"\tempUpdate.config", Dirs.ConfigUpdate);

                    string versionClientNew = GetLatestVersion();

                    if (SharedGlobals.ClientVersion != versionClientNew)
                    {
                        Log.Write("An update for BFBC2 Toolkit is available.");
                        MessageBox.Show("An update for BFBC2 Toolkit is available.\nVisit Nexus Mods to download the latest version.", "Update available");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                MessageBox.Show("Update failed! No internet connection? See error.log", "Error");
            }
        }

        private static string GetLatestVersion()
        {
            string versionClientNew = String.Empty;

            using (var xr = new XmlTextReader(Dirs.ConfigUpdate) as XmlReader)
            {
                while (xr.Read())
                {
                    while (xr.MoveToNextAttribute())
                    {
                        switch (xr.Name)
                        {
                            case "versionClient":
                                versionClientNew = xr.Value;
                                break;
                        }
                    }
                }
            }

            return versionClientNew;
        }
    }
}
