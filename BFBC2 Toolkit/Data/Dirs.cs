using System;

namespace BFBC2_Toolkit.Data
{
    public class Dirs
    {
        public static string filePath = "",
                             filesPathData = "",
                             selectedFilePathData = "",
                             selectedFileNameData = "",
                             filesPathMod = "",
                             selectedFilePathMod = "",
                             selectedFileNameMod = "",
                             modName = "";

        public static readonly string output = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output",
                                      outputHeightmap = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Heightmap",
                                      outputiTexture = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\iTexture",
                                      outputDDS = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\DDS",
                                      outputXML = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\XML",
                                      outputVideo = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Video",
                                      outputMods = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Mods",
                                      scripts = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts",
                                      scriptArchive = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\archive.pyw",
                                      scriptDBX = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\dbx.pyw",
                                      syntaxXML = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\syntaxXML.config",
                                      syntaxINI = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\syntaxINI.config",
                                      configGames = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\games.config",
                                      projects = Environment.CurrentDirectory + @"\BFBC2Toolkit\Projects",
                                      games = Environment.CurrentDirectory + @"\BFBC2Toolkit\Games",
                                      templateMod = Environment.CurrentDirectory + @"\BFBC2Toolkit\Templates\Mod",
                                      errorLog = Environment.CurrentDirectory + @"\BFBC2Toolkit\Logs\error.log",
                                      logs = Environment.CurrentDirectory + @"\BFBC2ModLoader\Logs";
    }
}
