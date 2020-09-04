using System;
using System.IO;
using System.Net;
using System.Windows;
using BFBC2ModLoader.Data;
using BFBC2Shared.Data;
using BFBC2Shared.Functions;

namespace BFBC2ModLoader.Functions
{
    public class Check
    {
        public static void Update()
        {
            try
            {
                string versionServerOld = Globals.VersionServer;

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Config/update.config", Dirs.ModsCommon + @"\tempUpdate.config");

                    if (File.Exists(Dirs.UpdateXML))
                        File.Delete(Dirs.UpdateXML);
                    if (File.Exists(Dirs.ModsCommon + @"\tempUpdate.config"))
                        File.Move(Dirs.ModsCommon + @"\tempUpdate.config", Dirs.UpdateXML);

                    Load.UpdateXML();

                    if (SharedGlobals.ClientVersion != Globals.VersionClientNew)
                    {
                        Log.Write("An update for BFBC2 Mod Loader is available.");

                        MessageBox.Show("An update for BFBC2 Mod Loader is available.\nVisit Nexus Mods to download the latest version.", "Update available");
                    }

                    if (versionServerOld != Globals.VersionServer)
                    {
                        Log.Write("Downloading updated lists for server and map browser...");

                        if (Globals.IsClient == true)
                        {
                            wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Loader/client/modded/bundleManifest", Dirs.ModsCommon + @"\tempBundleManifest");
                            wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Loader/client/modded/package.mft", Dirs.ModsCommon + @"\tempPackage.mft");
                        }
                        else
                        {
                            wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Loader/server/modded/bundleManifest", Dirs.ModsCommon + @"\tempBundleManifest");
                            wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Loader/server/modded/package.mft", Dirs.ModsCommon + @"\tempPackage.mft");
                        }

                        if (File.Exists(Dirs.BundleManiModded))
                            File.Delete(Dirs.BundleManiModded);
                        if (File.Exists(Dirs.ModsCommon + @"\tempBundleManifest"))
                            File.Move(Dirs.ModsCommon + @"\tempBundleManifest", Dirs.BundleManiModded);
                        if (File.Exists(Dirs.MftModded))
                            File.Delete(Dirs.MftModded);
                        if (File.Exists(Dirs.ModsCommon + @"\tempPackage.mft"))
                            File.Move(Dirs.ModsCommon + @"\tempPackage.mft", Dirs.MftModded);

                        if (Settings.ModsEnabled)
                        {
                            if (File.Exists(Dirs.MftRoot))
                                File.Delete(Dirs.MftRoot);
                            File.Copy(Dirs.MftModded, Dirs.MftRoot);
                            if (File.Exists(Dirs.BundleManiRoot))
                                File.Delete(Dirs.BundleManiRoot);
                            File.Copy(Dirs.BundleManiModded, Dirs.BundleManiRoot);
                        }

                        wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Config/news.config", Dirs.ModsCommon + @"\tempNews.config");

                        if (File.Exists(Dirs.LatestNewsXML))
                            File.Delete(Dirs.LatestNewsXML);
                        if (File.Exists(Dirs.ModsCommon + @"\tempNews.config"))
                            File.Move(Dirs.ModsCommon + @"\tempNews.config", Dirs.LatestNewsXML);

                        wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Config/listMaps.config", Dirs.ModsCommon + @"\tempListMaps.config");

                        if (File.Exists(Dirs.ListMapsXML))
                            File.Delete(Dirs.ListMapsXML);
                        if (File.Exists(Dirs.ModsCommon + @"\tempListMaps.config"))
                            File.Move(Dirs.ModsCommon + @"\tempListMaps.config", Dirs.ListMapsXML);

                        wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Config/listModes.config", Dirs.ModsCommon + @"\tempListModes.config");

                        if (File.Exists(Dirs.ListModesXML))
                            File.Delete(Dirs.ListModesXML);
                        if (File.Exists(Dirs.ModsCommon + @"\tempListModes.config"))
                            File.Move(Dirs.ModsCommon + @"\tempListModes.config", Dirs.ListModesXML);

                        wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Config/servers.config", Dirs.ModsCommon + @"\tempServers.config");

                        if (File.Exists(Dirs.ServersXML))
                            File.Delete(Dirs.ServersXML);
                        if (File.Exists(Dirs.ModsCommon + @"\tempServers.config"))
                            File.Move(Dirs.ModsCommon + @"\tempServers.config", Dirs.ServersXML);

                        if (Globals.IsClient == true)
                        {
                            wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Config/mapsClient.config", Dirs.ModsCommon + @"\tempMaps.config");
                        }
                        else
                        {
                            wc.DownloadFile("https://raw.githubusercontent.com/HeicoDev/FBOneTools/master/BFBC2ModLoader/BFBC2ModLoader/Config/mapsServer.config", Dirs.ModsCommon + @"\tempMaps.config");
                        }

                        if (File.Exists(Dirs.MapsXML))
                            File.Delete(Dirs.MapsXML);
                        if (File.Exists(Dirs.ModsCommon + @"\tempMaps.config"))
                            File.Move(Dirs.ModsCommon + @"\tempMaps.config", Dirs.MapsXML);

                        Log.Write("", "done");
                    }
                }             
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Update failed! No internet connection? See error.log", "error");
            }
        }

        public static void MapUpdate()
        {
            bool b = false;

            foreach (var item in UIElements.DataGridModManager.Items)
            {
                foreach (var item1 in UIElements.DataGridMapBrowser.Items)
                {
                    var itemMM = item as ModManagerItem;
                    var itemMB = item1 as MapBrowserItem;

                    if (itemMM.ModName == itemMB.MapName)
                    {
                        if (itemMM.ModVersion != itemMB.MapVersion)
                        {
                            b = true;
                            break;
                        }
                    }
                }

                if (b == true)
                {
                    break;
                }
            }

            if (b == true)
            {
                Globals.MapUpdatesAvailable = true;

                Log.Write("An update for one or more maps is available.");

                MessageBox.Show("An update for one or more maps is available.\nNavigate to the Map Browser to update the maps.", "Map updates available");
            }
        }
    }
}
