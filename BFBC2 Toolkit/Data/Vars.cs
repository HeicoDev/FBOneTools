using System.Collections.Generic;

namespace BFBC2_Toolkit.Data
{
    public class Vars
    {
        public static IDictionary<string, string> fbrbFiles = new Dictionary<string, string>();

        public static bool isDataTreeView = false,
                           isModAvailable = false,
                           isGameProfile = false;

        public static string textureFormat = "",
                             versionClient = "1.0.0";

        public static int textureWidth = 0,
                          textureHeight = 0;

        public static byte mipmapCount = 0;

        public static readonly string[] fileFormats = { "swfmovie", "dx10pixelshader", "havokphysicsdata", "treemeshset",
                                                        "terrainheightfield", "itexture", "animtreeinfo", "irradiancevolume",
                                                        "visualterrain", "skinnedmeshset", "dx10vertexshader", "aimanimation",
                                                        "occludermesh", "dx9shaderdatabase", "wave", "terrainmaterialmap",
                                                        "sootmesh", "rigidmeshset", "compositemeshset", "watermesh",
                                                        "visualwater", "dx9vertexshader", "dx9pixelshader", "dx11shaderdatabase",
                                                        "dx11pixelshader", "grannymodel", "ragdollresource", "grannyanimation",
                                                        "weathersystem", "dx11vertexshader", "terrain", "impulseresponse",
                                                        "binkmemory", "deltaanimation", "dx10shaderdatabase", "meshdata",
                                                        "xenonpixelshader", "xenonvertexshader", "xenontexture", "ps3pixelshader",
                                                        "ps3vertexshader", "ps3texture", "pathdatadefinition", "nonres",
                                                        "dbx", "bin", "dbmanifest", "ini", "txt" };

        public static readonly string[] supportedArchives = { "level-00.fbrb", "terrain-00.fbrb", "loader-00.fbrb", "ingame-00.fbrb",
                                                              "startup-00.fbrb", "mainmenu-00.fbrb" };

        public static readonly string[] gameIds = { "BFBC2-PC", "BFBC2-Server-PC", "BFBC2-PS3", "BFBC2-Xbox", "BFBC-PS3", "BFBC-Xbox",
                                                    "BF1943-PS3", "BF1943-Xbox" };

        public static void SetFbrbFiles()
        {
            fbrbFiles.Add("level-00.fbrb", "level-00 FbRB");
            fbrbFiles.Add("terrain-00.fbrb", "terrain-00 FbRB");
            fbrbFiles.Add("loader-00.fbrb", "loader-00 FbRB");
            fbrbFiles.Add("ingame-00.fbrb", "ingame-00 FbRB");
            fbrbFiles.Add("startup-00.fbrb", "startup-00 FbRB");
            fbrbFiles.Add("mainmenu-00.fbrb", "mainmenu-00 FbRB");
            fbrbFiles.Add("build_overlay-00.fbrb", "build_overlay-00 FbRB");
            fbrbFiles.Add("overlay-00.fbrb", "overlay-00 FbRB");
            fbrbFiles.Add("streaming_sounds-00.fbrb", "streaming_sounds-00 FbRB");
            fbrbFiles.Add("ondemand_sounds-00.fbrb", "ondemand_sounds-00 FbRB");
            fbrbFiles.Add("ondemand_awards-00.fbrb", "ondemand_awards-00 FbRB");
            fbrbFiles.Add("vo-00.fbrb", "vo-00 FbRB");
            fbrbFiles.Add("streaming_vo_en-00.fbrb", "streaming_vo_en-00 FbRB");
            fbrbFiles.Add("streaming_vo_de-00.fbrb", "streaming_vo_de-00 FbRB");
            fbrbFiles.Add("streaming_vo_ru-00.fbrb", "streaming_vo_ru-00 FbRB");
            fbrbFiles.Add("streaming_vo_pl-00.fbrb", "streaming_vo_pl-00 FbRB");
            fbrbFiles.Add("streaming_vo_es-00.fbrb", "streaming_vo_es-00 FbRB");
            fbrbFiles.Add("streaming_vo_fr-00.fbrb", "streaming_vo_fr-00 FbRB");
            fbrbFiles.Add("streaming_vo_it-00.fbrb", "streaming_vo_it-00 FbRB");
            fbrbFiles.Add("en-00.fbrb", "en-00 FbRB");
            fbrbFiles.Add("de-00.fbrb", "de-00 FbRB");
            fbrbFiles.Add("ru-00.fbrb", "ru-00 FbRB");
            fbrbFiles.Add("pl-00.fbrb", "pl-00 FbRB");
            fbrbFiles.Add("es-00.fbrb", "es-00 FbRB");
            fbrbFiles.Add("fr-00.fbrb", "fr-00 FbRB");
            fbrbFiles.Add("it-00.fbrb", "it-00 FbRB");
            fbrbFiles.Add("eula_en-00.fbrb", "eula_en-00 FbRB");
            fbrbFiles.Add("eula_de-00.fbrb", "eula_de-00 FbRB");
            fbrbFiles.Add("eula_ru-00.fbrb", "eula_ru-00 FbRB");
            fbrbFiles.Add("eula_pl-00.fbrb", "eula_pl-00 FbRB");
            fbrbFiles.Add("eula_es-00.fbrb", "eula_es-00 FbRB");
            fbrbFiles.Add("eula_fr-00.fbrb", "eula_fr-00 FbRB");
            fbrbFiles.Add("eula_it-00.fbrb", "eula_it-00 FbRB");
            fbrbFiles.Add("eula_jp-00.fbrb", "eula_jp-00 FbRB");
            fbrbFiles.Add("fonts_en-00.fbrb", "fonts_en-00 FbRB");
            fbrbFiles.Add("fonts_de-00.fbrb", "fonts_de-00 FbRB");
            fbrbFiles.Add("fonts_ru-00.fbrb", "fonts_ru-00 FbRB");
            fbrbFiles.Add("fonts_pl-00.fbrb", "fonts_pl-00 FbRB");
            fbrbFiles.Add("fonts_es-00.fbrb", "fonts_es-00 FbRB");
            fbrbFiles.Add("fonts_fr-00.fbrb", "fonts_fr-00 FbRB");
            fbrbFiles.Add("fonts_it-00.fbrb", "fonts_it-00 FbRB");
            fbrbFiles.Add("fonts_jp-00.fbrb", "fonts_jp-00 FbRB");
            fbrbFiles.Add("fonts_sd_en-00.fbrb", "fonts_sd_en-00 FbRB");
            fbrbFiles.Add("fonts_sd_de-00.fbrb", "fonts_sd_de-00 FbRB");
            fbrbFiles.Add("fonts_sd_ru-00.fbrb", "fonts_sd_ru-00 FbRB");
            fbrbFiles.Add("fonts_sd_pl-00.fbrb", "fonts_sd_pl-00 FbRB");
            fbrbFiles.Add("fonts_sd_es-00.fbrb", "fonts_sd_es-00 FbRB");
            fbrbFiles.Add("fonts_sd_fr-00.fbrb", "fonts_sd_fr-00 FbRB");
            fbrbFiles.Add("fonts_sd_it-00.fbrb", "fonts_sd_it-00 FbRB");
            fbrbFiles.Add("fonts_sd_jp-00.fbrb", "fonts_sd_jp-00 FbRB");
        }
    }
}
