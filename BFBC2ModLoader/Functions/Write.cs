using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using BFBC2ModLoader.Data;
using BFBC2ModLoader.Data.Bindings;
using BFBC2Shared.Functions;

namespace BFBC2ModLoader.Functions
{
    public class Write
    {
        private static string ModLink;

        public static void ModInfo()
        {
            var item = UIElements.DataGridModManager.SelectedItem as ModManagerItem;
            int length = item.ModAuthor.Length;

            if (item.ModLink.Length > length)
                length = item.ModLink.Length;
            if (item.ModMapID.Length > length)
                length = item.ModMapID.Length;
            if (item.ModName.Length > length)
                length = item.ModName.Length;
            if (item.ModVersion.Length > length)
                length = item.ModVersion.Length;

            var para = new Paragraph();

            UIElements.TxtBoxModInfo.Document.Blocks.Clear();
            UIElements.TxtBoxModInfo.Document.PageWidth = length * 8;
            if (item.ModMapID == "")
                UIElements.TxtBoxModInfo.AppendText("Name:\r" + item.ModName + "\r\rVersion:\r" + item.ModVersion + "\r\rAuthor:\r" + item.ModAuthor + "\r\rType:\r" + item.ModType + "\r\rMapID:\rNone\r\rLink:");
            else
                UIElements.TxtBoxModInfo.AppendText("Name:\r" + item.ModName + "\r\rVersion:\r" + item.ModVersion + "\r\rAuthor:\r" + item.ModAuthor + "\r\rType:\r" + item.ModType + "\r\rMapID:\r" + item.ModMapID + "\r\rLink:");
            UIElements.TxtBoxModInfo.Document.Blocks.Add(para);
            UIElements.TxtBoxModInfo.ScrollToHome();

            if (item.ModLink != String.Empty)
            {
                var link = new Hyperlink();
                link.IsEnabled = true;
                link.Inlines.Add(item.ModLink);
                link.NavigateUri = new Uri(item.ModLink);
                link.Click += new RoutedEventHandler(Link_Click);
                para.Inlines.Add(link);

                ModLink = item.ModLink;
            }
        }

        private static void Link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(ModLink);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open link! See error.log", "error");
            }
        }

        public static void ServerInfo()
        {
            var item = UIElements.DataGridServerBrowser.SelectedItem as ServerBrowserItem;
            int length = item.ServerMap.Length;

            if (item.ServerMode.Length > length)
                length = item.ServerMode.Length;
            if (item.ServerName.Length > length)
                length = item.ServerName.Length;
            if (item.ServerReq.Length > length)
                length = item.ServerReq.Length;

            UIElements.TxtBoxServerInfo.Document.Blocks.Clear();
            UIElements.TxtBoxServerInfo.Document.PageWidth = length * 8;
            UIElements.TxtBoxServerInfo.AppendText("Name:\r" + item.ServerName + "\r\rMap:\r" + item.ServerMap + "\r\rMode:\r" + item.ServerMode + "\r\rBackend:\r" + item.ServerBackend + "\r\rPlayers:\r" + item.ServerPlayers + "\r\rPing:\r" + item.ServerPing + "\r\rRequirements:" + item.ServerReq);
            UIElements.TxtBoxServerInfo.ScrollToHome();
        }

        public static void MapInfo(string isInstalled)
        {
            var item = UIElements.DataGridMapBrowser.SelectedItem as MapBrowserItem;
            int length = item.MapAuthor.Length;

            if (item.MapName.Length > length)
                length = item.MapName.Length;
            if (item.MapVersion.Length > length)
                length = item.MapVersion.Length;
            if (item.MapReq.Length > length)
                length = item.MapReq.Length;

            UIElements.TxtBoxMapInfo.Document.Blocks.Clear();
            UIElements.TxtBoxMapInfo.Document.PageWidth = length * 8;
            UIElements.TxtBoxMapInfo.AppendText("Name:\r" + item.MapName + "\r\rVersion:\r" + item.MapVersion + "\r\rAuthor:\r" + item.MapAuthor + "\r\rSize:\r" + item.MapSize + "\r\rInstalled:\r" + isInstalled + "\r\rRequirements:" + item.MapReq);
            UIElements.TxtBoxMapInfo.ScrollToHome();
        }
    }
}
