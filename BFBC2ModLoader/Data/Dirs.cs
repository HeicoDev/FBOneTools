using System;
using System.Collections.Generic;
using BFBC2Shared.Data;

namespace BFBC2ModLoader.Data
{
    public class Dirs
    {
        public static IDictionary<string, string> FbrbDirs { get; } = new Dictionary<string, string>();
        public static IDictionary<string, string> FbrbFiles { get; } = new Dictionary<string, string>();

        public static string ExtractPath { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Workspace";
        public static string TempText { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Temp.txt";
        public static string ModsText { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Mods.txt";
        public static string ModsFolder { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Mods";
        public static string ModsXML { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\mods.config";
        public static string ConfigXML { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\settings.config";
        public static string MapsXML { get; set; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\mapsClient.config";
        public static string ServersXML { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\servers.config";
        public static string ListMapsXML { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\listMaps.config";
        public static string ListModesXML { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\listModes.config";
        public static string LatestNewsXML { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\news.config";
        public static string UpdateXML { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\update.config";
        public static string ScriptServer { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Scripts\server.pyw";
        public static string ScriptArchive { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Scripts\fbrb.pyw";
        public static string ServerInfo { get; } = Environment.CurrentDirectory + @"\serverInfo.txt";
        public static string TempServer { get; } = Environment.CurrentDirectory + @"\tempServer.txt";
        public static string ModsCommon { get; } = Environment.CurrentDirectory + @"\package\mods";
        public static string LevelsPathPackage { get; set; } = Environment.CurrentDirectory + @"\package\levels";
        public static string LevelsPathDist { get; } = Environment.CurrentDirectory + @"\dist\win32\levels";
        public static string StartupPath { get; } = Environment.CurrentDirectory;
        public static string MftRoot { get; set; } = Environment.CurrentDirectory + @"\dist\win32\package.mft";
        public static string MftOriginal { get; set; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\client\original\package.mft";
        public static string MftModded { get; set; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\client\modded\package.mft";
        public static string BundleManiRoot { get; set; } = Environment.CurrentDirectory + @"\dist\win32\bundleManifest";
        public static string BundleManiOriginal { get; set; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\client\original\bundleManifest";
        public static string BundleManiModded { get; set; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\client\modded\bundleManifest";
        public static string MapZIP { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Download\map.zip";
        public static string LogoPng { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Resources\Images\logo.png";        
        public static string Logs { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Logs";
        public static string Downloads { get; } = Environment.CurrentDirectory + @"\BFBC2ModLoader\Download";

        public static void SetSharedVars()
        {
            SharedDirs.ErrorLog = Environment.CurrentDirectory + @"\BFBC2ModLoader\Logs\error.log";
        }

        public static void AddFbrbDirs()
        {
            FbrbDirs.Add("level-00 FbRB", Environment.CurrentDirectory + @"\package\mods\level-00 FbRB");
            FbrbDirs.Add("terrain-00 FbRB", Environment.CurrentDirectory + @"\package\mods\terrain-00 FbRB");
            FbrbDirs.Add("loader-00 FbRB", Environment.CurrentDirectory + @"\package\mods\loader-00 FbRB");
            FbrbDirs.Add("ingame-00 FbRB", Environment.CurrentDirectory + @"\package\mods\ingame-00 FbRB");
            FbrbDirs.Add("startup-00 FbRB", Environment.CurrentDirectory + @"\package\mods\startup-00 FbRB");
            FbrbDirs.Add("mainmenu-00 FbRB", Environment.CurrentDirectory + @"\package\mods\mainmenu-00 FbRB");
            FbrbDirs.Add("build_overlay-00 FbRB", Environment.CurrentDirectory + @"\package\mods\build_overlay-00 FbRB");
            FbrbDirs.Add("overlay-00 FbRB", Environment.CurrentDirectory + @"\package\mods\overlay-00 FbRB");
            FbrbDirs.Add("streaming_sounds-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_sounds-00 FbRB");
            FbrbDirs.Add("ondemand_sounds-00 FbRB", Environment.CurrentDirectory + @"\package\mods\ondemand_sounds-00 FbRB");
            FbrbDirs.Add("ondemand_awards-00 FbRB", Environment.CurrentDirectory + @"\package\mods\ondemand_awards-00 FbRB");
            FbrbDirs.Add("vo-00 FbRB", Environment.CurrentDirectory + @"\package\mods\vo-00 FbRB");
            FbrbDirs.Add("streaming_vo_en-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_vo_en-00 FbRB");
            FbrbDirs.Add("streaming_vo_de-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_vo_de-00 FbRB");
            FbrbDirs.Add("streaming_vo_ru-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_vo_ru-00 FbRB");
            FbrbDirs.Add("streaming_vo_pl-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_vo_pl-00 FbRB");
            FbrbDirs.Add("streaming_vo_es-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_vo_es-00 FbRB");
            FbrbDirs.Add("streaming_vo_fr-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_vo_fr-00 FbRB");
            FbrbDirs.Add("streaming_vo_it-00 FbRB", Environment.CurrentDirectory + @"\package\mods\streaming_vo_it-00 FbRB");
            FbrbDirs.Add("en-00 FbRB", Environment.CurrentDirectory + @"\package\mods\en-00 FbRB");
            FbrbDirs.Add("de-00 FbRB", Environment.CurrentDirectory + @"\package\mods\de-00 FbRB");
            FbrbDirs.Add("ru-00 FbRB", Environment.CurrentDirectory + @"\package\mods\ru-00 FbRB");
            FbrbDirs.Add("pl-00 FbRB", Environment.CurrentDirectory + @"\package\mods\pl-00 FbRB");
            FbrbDirs.Add("es-00 FbRB", Environment.CurrentDirectory + @"\package\mods\es-00 FbRB");
            FbrbDirs.Add("fr-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fr-00 FbRB");
            FbrbDirs.Add("it-00 FbRB", Environment.CurrentDirectory + @"\package\mods\it-00 FbRB");
            FbrbDirs.Add("eula_en-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_en-00 FbRB");
            FbrbDirs.Add("eula_de-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_de-00 FbRB");
            FbrbDirs.Add("eula_ru-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_ru-00 FbRB");
            FbrbDirs.Add("eula_pl-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_pl-00 FbRB");
            FbrbDirs.Add("eula_es-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_es-00 FbRB");
            FbrbDirs.Add("eula_fr-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_fr-00 FbRB");
            FbrbDirs.Add("eula_it-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_it-00 FbRB");
            FbrbDirs.Add("eula_jp-00 FbRB", Environment.CurrentDirectory + @"\package\mods\eula_jp-00 FbRB");
            FbrbDirs.Add("fonts_en-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_en-00 FbRB");
            FbrbDirs.Add("fonts_de-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_de-00 FbRB");
            FbrbDirs.Add("fonts_ru-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_ru-00 FbRB");
            FbrbDirs.Add("fonts_pl-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_pl-00 FbRB");
            FbrbDirs.Add("fonts_es-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_es-00 FbRB");
            FbrbDirs.Add("fonts_fr-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_fr-00 FbRB");
            FbrbDirs.Add("fonts_it-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_it-00 FbRB");
            FbrbDirs.Add("fonts_jp-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_jp-00 FbRB");
            FbrbDirs.Add("fonts_sd_en-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_en-00 FbRB");
            FbrbDirs.Add("fonts_sd_de-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_de-00 FbRB");
            FbrbDirs.Add("fonts_sd_ru-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_ru-00 FbRB");
            FbrbDirs.Add("fonts_sd_pl-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_pl-00 FbRB");
            FbrbDirs.Add("fonts_sd_es-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_es-00 FbRB");
            FbrbDirs.Add("fonts_sd_fr-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_fr-00 FbRB");
            FbrbDirs.Add("fonts_sd_it-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_it-00 FbRB");
            FbrbDirs.Add("fonts_sd_jp-00 FbRB", Environment.CurrentDirectory + @"\package\mods\fonts_sd_jp-00 FbRB");

            FbrbFiles.Add("level-00.fbrb", Environment.CurrentDirectory + @"\package\mods\level-00.fbrb");
            FbrbFiles.Add("terrain-00.fbrb", Environment.CurrentDirectory + @"\package\mods\terrain-00.fbrb");
            FbrbFiles.Add("loader-00.fbrb", Environment.CurrentDirectory + @"\package\mods\loader-00.fbrb");
            FbrbFiles.Add("ingame-00.fbrb", Environment.CurrentDirectory + @"\package\mods\ingame-00.fbrb");
            FbrbFiles.Add("startup-00.fbrb", Environment.CurrentDirectory + @"\package\mods\startup-00.fbrb");
            FbrbFiles.Add("mainmenu-00.fbrb", Environment.CurrentDirectory + @"\package\mods\mainmenu-00.fbrb");
            FbrbFiles.Add("build_overlay-00.fbrb", Environment.CurrentDirectory + @"\package\mods\build_overlay-00.fbrb");
            FbrbFiles.Add("overlay-00.fbrb", Environment.CurrentDirectory + @"\package\mods\overlay-00.fbrb");
            FbrbFiles.Add("streaming_sounds-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_sounds-00.fbrb");
            FbrbFiles.Add("ondemand_sounds-00.fbrb", Environment.CurrentDirectory + @"\package\mods\ondemand_sounds-00.fbrb");
            FbrbFiles.Add("ondemand_awards-00.fbrb", Environment.CurrentDirectory + @"\package\mods\ondemand_awards-00.fbrb");
            FbrbFiles.Add("vo-00.fbrb", Environment.CurrentDirectory + @"\package\mods\vo-00.fbrb");
            FbrbFiles.Add("streaming_vo_en-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_vo_en-00.fbrb");
            FbrbFiles.Add("streaming_vo_de-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_vo_de-00.fbrb");
            FbrbFiles.Add("streaming_vo_ru-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_vo_ru-00.fbrb");
            FbrbFiles.Add("streaming_vo_pl-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_vo_pl-00.fbrb");
            FbrbFiles.Add("streaming_vo_es-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_vo_es-00.fbrb");
            FbrbFiles.Add("streaming_vo_fr-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_vo_fr-00.fbrb");
            FbrbFiles.Add("streaming_vo_it-00.fbrb", Environment.CurrentDirectory + @"\package\mods\streaming_vo_it-00.fbrb");
            FbrbFiles.Add("en-00.fbrb", Environment.CurrentDirectory + @"\package\mods\en-00.fbrb");
            FbrbFiles.Add("de-00.fbrb", Environment.CurrentDirectory + @"\package\mods\de-00.fbrb");
            FbrbFiles.Add("ru-00.fbrb", Environment.CurrentDirectory + @"\package\mods\ru-00.fbrb");
            FbrbFiles.Add("pl-00.fbrb", Environment.CurrentDirectory + @"\package\mods\pl-00.fbrb");
            FbrbFiles.Add("es-00.fbrb", Environment.CurrentDirectory + @"\package\mods\es-00.fbrb");
            FbrbFiles.Add("fr-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fr-00.fbrb");
            FbrbFiles.Add("it-00.fbrb", Environment.CurrentDirectory + @"\package\mods\it-00.fbrb");
            FbrbFiles.Add("eula_en-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_en-00.fbrb");
            FbrbFiles.Add("eula_de-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_de-00.fbrb");
            FbrbFiles.Add("eula_ru-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_ru-00.fbrb");
            FbrbFiles.Add("eula_pl-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_pl-00.fbrb");
            FbrbFiles.Add("eula_es-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_es-00.fbrb");
            FbrbFiles.Add("eula_fr-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_fr-00.fbrb");
            FbrbFiles.Add("eula_it-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_it-00.fbrb");
            FbrbFiles.Add("eula_jp-00.fbrb", Environment.CurrentDirectory + @"\package\mods\eula_jp-00.fbrb");
            FbrbFiles.Add("fonts_en-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_en-00.fbrb");
            FbrbFiles.Add("fonts_de-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_de-00.fbrb");
            FbrbFiles.Add("fonts_ru-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_ru-00.fbrb");
            FbrbFiles.Add("fonts_pl-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_pl-00.fbrb");
            FbrbFiles.Add("fonts_es-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_es-00.fbrb");
            FbrbFiles.Add("fonts_fr-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_fr-00.fbrb");
            FbrbFiles.Add("fonts_it-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_it-00.fbrb");
            FbrbFiles.Add("fonts_jp-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_jp-00.fbrb");
            FbrbFiles.Add("fonts_sd_en-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_en-00.fbrb");
            FbrbFiles.Add("fonts_sd_de-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_de-00.fbrb");
            FbrbFiles.Add("fonts_sd_ru-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_ru-00.fbrb");
            FbrbFiles.Add("fonts_sd_pl-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_pl-00.fbrb");
            FbrbFiles.Add("fonts_sd_es-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_es-00.fbrb");
            FbrbFiles.Add("fonts_sd_fr-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_fr-00.fbrb");
            FbrbFiles.Add("fonts_sd_it-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_it-00.fbrb");
            FbrbFiles.Add("fonts_sd_jp-00.fbrb", Environment.CurrentDirectory + @"\package\mods\fonts_sd_jp-00.fbrb");
        }

        public static void SwitchToServer()
        {
            //Change to server directories
            MapsXML = Environment.CurrentDirectory + @"\BFBC2ModLoader\Config\mapsServer.config";
            LevelsPathPackage = Environment.CurrentDirectory + @"\dist\linux\levels";
            MftRoot = Environment.CurrentDirectory + @"\dist\linux\package.mft";
            MftOriginal = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\server\original\package.mft";
            MftModded = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\server\modded\package.mft";
            BundleManiRoot = Environment.CurrentDirectory + @"\dist\linux\bundleManifest";
            BundleManiOriginal = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\server\original\bundleManifest";
            BundleManiModded = Environment.CurrentDirectory + @"\BFBC2ModLoader\Loader\server\modded\bundleManifest";
        }
    }
}
