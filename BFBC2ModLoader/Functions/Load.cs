using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml;
using BFBC2ModLoader.Data;
using BFBC2ModLoader.Data.Bindings;
using BFBC2Shared.Functions;

namespace BFBC2ModLoader.Functions
{
    public class Load
    {
        private static string latestChangelog;
        private static string latestNews;

        public static void NewsXML()
        {
            try
            {
                using (var xr = new XmlTextReader(Dirs.LatestNewsXML) as XmlReader)
                {
                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            switch (xr.Name)
                            {
                                case "Changelog":
                                    latestChangelog = xr.Value;
                                    break;
                                case "News":
                                    latestNews = xr.Value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not load news.config! See error.log", "error");
            }
        }

        public static void UpdateXML()
        {
            try
            {
                using (var xr = new XmlTextReader(Dirs.UpdateXML) as XmlReader)
                {
                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            switch (xr.Name)
                            {
                                case "versionClient":
                                    Globals.VersionClientNew = xr.Value;
                                    break;
                                case "versionServer":
                                    Globals.VersionServer = xr.Value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not load update.config! See error.log", "error");
            }
        }

        public static void ModsXML()
        {
            try
            {
                using (var xr = new XmlTextReader(Dirs.ModsXML) as XmlReader)
                {
                    UIElements.DataGridModManager.Items.Clear();

                    string mOrder = "",
                           mName = "",
                           mVersion = "",
                           mType = "";

                    bool mEnabled = true;

                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            switch (xr.Name)
                            {
                                case "Order":
                                    mOrder = xr.Value;
                                    break;
                                case "Enabled":
                                    if (xr.Value == "True")
                                        mEnabled = true;
                                    else
                                        mEnabled = false;
                                    break;
                                case "Name":
                                    mName = xr.Value;
                                    break;
                                case "Version":
                                    mVersion = xr.Value;
                                    break;
                                case "Type":
                                    mType = xr.Value;
                                    break;
                            }

                            if (mType != "")
                            {
                                var modInfo = new IniFile(Dirs.ModsFolder + @"\" + mName + @"\ModInfo.ini");

                                string mAuthor = modInfo.Read("Author", "ModInfo"),
                                       mMapID = modInfo.Read("MapID", "ModInfo"),
                                       mImage = modInfo.Read("Image", "ModInfo"),
                                       mLink = modInfo.Read("Link", "ModInfo");

                                UIElements.DataGridModManager.Items.Add(new ModManagerItem() { ModOrder = mOrder, ModEnabled = mEnabled, ModName = mName, ModVersion = mVersion, ModAuthor = mAuthor, ModType = mType, ModMapID = mMapID, ModImage = mImage, ModLink = mLink });
                                mOrder = "";
                                mName = "";
                                mVersion = "";
                                mType = "";
                            }
                        }
                    }
                }

                Misc.OrderNumber();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not load mods.config! See error.log", "error");
            }
        }

        public static bool ConfigXML()
        {
            try
            {
                using (var xr = new XmlTextReader(Dirs.ConfigXML) as XmlReader)
                {
                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            switch (xr.Name)
                            {
                                case "modsEnabled":
                                    Settings.ModsEnabled = Convert.ToBoolean(xr.Value);
                                    break;
                                case "autoUpdateCheckEnabled":
                                    Settings.IsAutoUpdateCheckEnabled = Convert.ToBoolean(xr.Value);
                                    break;
                                case "pathToPython":
                                    Settings.PathToPython = xr.Value;
                                    break;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not load settings.config! See error.log", "error");

                return true;
            }
        }

        public static void MapsXML()
        {
            try
            {
                using (var xr = new XmlTextReader(Dirs.MapsXML) as XmlReader)
                {
                    UIElements.DataGridMapBrowser.Items.Clear();

                    string mName = "",
                           mVersion = "",
                           mSize = "",
                           mAuthor = "",
                           mLink = "",
                           mImage = "",
                           mReq = "";

                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            switch (xr.Name)
                            {
                                case "Name":
                                    mName = xr.Value;
                                    break;
                                case "Version":
                                    mVersion = xr.Value;
                                    break;
                                case "Size":
                                    mSize = xr.Value;
                                    break;
                                case "Author":
                                    mAuthor = xr.Value;
                                    break;
                                case "Link":
                                    mLink = xr.Value;
                                    break;
                                case "Image":
                                    mImage = xr.Value;
                                    break;
                                case "Req":
                                    mReq = xr.Value;
                                    break;
                            }

                            if (mReq != "")
                            {
                                UIElements.DataGridMapBrowser.Items.Add(new MapBrowserItem() { MapName = mName, MapVersion = mVersion, MapSize = mSize, MapAuthor = mAuthor, MapLink = mLink, MapImage = mImage, MapReq = mReq });
                                mName = "";
                                mVersion = "";
                                mSize = "";
                                mAuthor = "";
                                mLink = "";
                                mImage = "";
                                mReq = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not load maps.config! See error.log", "error");
            }
        }

        public static async Task ServersXML()
        {
            try
            {
                using (var xr = new XmlTextReader(Dirs.ServersXML) as XmlReader)
                {
                    UIElements.DataGridServerBrowser.Items.Clear();

                    string sName = "",
                           sMap = "",
                           sMode = "",
                           sBackend = "",
                           sPlayers = "",
                           sReq = "",
                           sIP = "",
                           sPort = "",
                           sPing = "";

                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            switch (xr.Name)
                            {
                                case "Backend":
                                    sBackend = xr.Value;
                                    break;
                                case "IP":
                                    sIP = xr.Value;

                                    var p = new Ping();
                                    var pingReply = p.Send(sIP, 1000);

                                    if (pingReply.Status == IPStatus.Success)
                                        sPing = pingReply.RoundtripTime.ToString();
                                    break;
                                case "Port":
                                    sPort = xr.Value;

                                    using (var sw = new StreamWriter(Dirs.TempServer))
                                        sw.WriteLine(sIP + Environment.NewLine + sPort);

                                    var process = Process.Start(Settings.PathToPython, "\"" + Dirs.ScriptServer + "\"");
                                    await Task.Run(() => process.WaitForExit());
                                    process.Close();

                                    if (File.Exists(Dirs.ServerInfo))
                                    {
                                        string[] sInfo = File.ReadAllLines(Dirs.ServerInfo);

                                        sName = sInfo[1];
                                        sMap = sInfo[5];
                                        sMode = sInfo[4];
                                        sPlayers = sInfo[2] + "/" + sInfo[3];

                                        using (var xr1 = new XmlTextReader(Dirs.ListMapsXML) as XmlReader)
                                        {
                                            while (xr1.Read())
                                            {
                                                while (xr1.MoveToNextAttribute())
                                                {
                                                    if (sMap.Contains(xr1.Name))
                                                        sMap = xr1.Value;
                                                }
                                            }
                                        }
                                    }
                                    if (File.Exists(Dirs.TempServer))
                                        File.Delete(Dirs.TempServer);
                                    if (File.Exists(Dirs.ServerInfo))
                                        File.Delete(Dirs.ServerInfo);
                                    break;
                                case "Req":
                                    sReq = xr.Value;
                                    break;
                            }

                            if (sReq != "" && sPlayers != "")
                            {
                                UIElements.DataGridServerBrowser.Items.Add(new ServerBrowserItem() { ServerName = sName, ServerMap = sMap, ServerMode = sMode, ServerBackend = sBackend, ServerPlayers = sPlayers, ServerPing = sPing, ServerReq = sReq });
                                sName = "";
                                sMap = "";
                                sMode = "";
                                sBackend = "";
                                sPlayers = "";
                                sReq = "";
                                sIP = "";
                                sPort = "";
                                sPing = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not load servers.config! See error.log", "error");
            }
        }
    }
}
