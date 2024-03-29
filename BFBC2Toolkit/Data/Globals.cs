﻿using System.Collections.Generic;
using BFBC2Toolkit.Data.Files;
using BFBC2Shared.Data;

namespace BFBC2Toolkit.Data
{
    public class Globals
    {
        public static IDictionary<string, string> FbrbFiles { get; } = new Dictionary<string, string>();

        public static bool IsGameProfile { get; set; } = false;
        public static bool IsDataTreeView { get; set; } = false;
        public static bool IsDataAvailable { get; set; } = false;    
        public static bool IsModAvailable { get; set; } = false;

        public static Texture SelectedTexture { get; set; }

        public static string[] FileFormats { get; } = { "swfmovie", "dx10pixelshader", "havokphysicsdata", "treemeshset",
                                                        "terrainheightfield", "itexture", "animtreeinfo", "irradiancevolume",
                                                        "visualterrain", "skinnedmeshset", "dx10vertexshader", "aimanimation",
                                                        "occludermesh", "dx9shaderdatabase", "wave", "terrainmaterialmap",
                                                        "sootmesh", "rigidmeshset", "compositemeshset", "watermesh",
                                                        "visualwater", "dx9vertexshader", "dx9pixelshader", "dx11shaderdatabase",
                                                        "dx11pixelshader", "grannymodel", "ragdollresource", "grannyanimation",
                                                        "weathersystem", "dx11vertexshader", "terrain", "impulseresponse",
                                                        "binkmemory", "deltaanimation", "dx10shaderdatabase", "meshdata",
                                                        "xenonpixelshader", "xenonvertexshader", "xenonshaderdatabase", "xenontexture", "ps3pixelshader",
                                                        "ps3vertexshader", "ps3shaderdatabase", "ps3texture", "pathdatadefinition", "dbxdeleted", "resdeleted",
                                                        "nonres", "res", "dbx", "bin", "dbmanifest", "ini", "txt" };
        
        public static string[] GameIds { get; } = { "BFBC2-PC", "BFBC2-Server-PC", "BFBC2-PS3", "BFBC2-Xbox", "BFBC-PS3", "BFBC-Xbox",
                                                    "BF1943-PS3", "BF1943-Xbox" };

        public static void SetSharedVars()
        {
            SharedGlobals.ClientName = "BFBC2 Toolkit";
            SharedGlobals.ClientVersion = "1.0.3";
        }

        public static void SetFbrbFiles()
        {
            FbrbFiles.Add("level-00.fbrb", "level-00 FbRB");
            FbrbFiles.Add("terrain-00.fbrb", "terrain-00 FbRB");
            FbrbFiles.Add("loader-00.fbrb", "loader-00 FbRB");
            FbrbFiles.Add("ingame-00.fbrb", "ingame-00 FbRB");
            FbrbFiles.Add("startup-00.fbrb", "startup-00 FbRB");
            FbrbFiles.Add("mainmenu-00.fbrb", "mainmenu-00 FbRB");
            FbrbFiles.Add("build_overlay-00.fbrb", "build_overlay-00 FbRB");
            FbrbFiles.Add("overlay-00.fbrb", "overlay-00 FbRB");
            FbrbFiles.Add("streaming_sounds-00.fbrb", "streaming_sounds-00 FbRB");
            FbrbFiles.Add("ondemand_sounds-00.fbrb", "ondemand_sounds-00 FbRB");
            FbrbFiles.Add("ondemand_awards-00.fbrb", "ondemand_awards-00 FbRB");
            FbrbFiles.Add("vo-00.fbrb", "vo-00 FbRB");
            FbrbFiles.Add("streaming_vo_en-00.fbrb", "streaming_vo_en-00 FbRB");
            FbrbFiles.Add("streaming_vo_de-00.fbrb", "streaming_vo_de-00 FbRB");
            FbrbFiles.Add("streaming_vo_ru-00.fbrb", "streaming_vo_ru-00 FbRB");
            FbrbFiles.Add("streaming_vo_pl-00.fbrb", "streaming_vo_pl-00 FbRB");
            FbrbFiles.Add("streaming_vo_es-00.fbrb", "streaming_vo_es-00 FbRB");
            FbrbFiles.Add("streaming_vo_fr-00.fbrb", "streaming_vo_fr-00 FbRB");
            FbrbFiles.Add("streaming_vo_it-00.fbrb", "streaming_vo_it-00 FbRB");
            FbrbFiles.Add("en-00.fbrb", "en-00 FbRB");
            FbrbFiles.Add("de-00.fbrb", "de-00 FbRB");
            FbrbFiles.Add("ru-00.fbrb", "ru-00 FbRB");
            FbrbFiles.Add("pl-00.fbrb", "pl-00 FbRB");
            FbrbFiles.Add("es-00.fbrb", "es-00 FbRB");
            FbrbFiles.Add("fr-00.fbrb", "fr-00 FbRB");
            FbrbFiles.Add("it-00.fbrb", "it-00 FbRB");
            FbrbFiles.Add("eula_en-00.fbrb", "eula_en-00 FbRB");
            FbrbFiles.Add("eula_de-00.fbrb", "eula_de-00 FbRB");
            FbrbFiles.Add("eula_ru-00.fbrb", "eula_ru-00 FbRB");
            FbrbFiles.Add("eula_pl-00.fbrb", "eula_pl-00 FbRB");
            FbrbFiles.Add("eula_es-00.fbrb", "eula_es-00 FbRB");
            FbrbFiles.Add("eula_fr-00.fbrb", "eula_fr-00 FbRB");
            FbrbFiles.Add("eula_it-00.fbrb", "eula_it-00 FbRB");
            FbrbFiles.Add("eula_jp-00.fbrb", "eula_jp-00 FbRB");
            FbrbFiles.Add("fonts_en-00.fbrb", "fonts_en-00 FbRB");
            FbrbFiles.Add("fonts_de-00.fbrb", "fonts_de-00 FbRB");
            FbrbFiles.Add("fonts_ru-00.fbrb", "fonts_ru-00 FbRB");
            FbrbFiles.Add("fonts_pl-00.fbrb", "fonts_pl-00 FbRB");
            FbrbFiles.Add("fonts_es-00.fbrb", "fonts_es-00 FbRB");
            FbrbFiles.Add("fonts_fr-00.fbrb", "fonts_fr-00 FbRB");
            FbrbFiles.Add("fonts_it-00.fbrb", "fonts_it-00 FbRB");
            FbrbFiles.Add("fonts_jp-00.fbrb", "fonts_jp-00 FbRB");
            FbrbFiles.Add("fonts_sd_en-00.fbrb", "fonts_sd_en-00 FbRB");
            FbrbFiles.Add("fonts_sd_de-00.fbrb", "fonts_sd_de-00 FbRB");
            FbrbFiles.Add("fonts_sd_ru-00.fbrb", "fonts_sd_ru-00 FbRB");
            FbrbFiles.Add("fonts_sd_pl-00.fbrb", "fonts_sd_pl-00 FbRB");
            FbrbFiles.Add("fonts_sd_es-00.fbrb", "fonts_sd_es-00 FbRB");
            FbrbFiles.Add("fonts_sd_fr-00.fbrb", "fonts_sd_fr-00 FbRB");
            FbrbFiles.Add("fonts_sd_it-00.fbrb", "fonts_sd_it-00 FbRB");
            FbrbFiles.Add("fonts_sd_jp-00.fbrb", "fonts_sd_jp-00 FbRB");
        }
    }
}
