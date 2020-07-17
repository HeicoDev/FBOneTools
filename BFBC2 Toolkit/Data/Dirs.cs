using System;

namespace BFBC2_Toolkit.Data
{
    public class Dirs
    {
        public static string ModName { get; set; }
        public static string FilePath { get; set; }
        public static string FilesPathData { get; set; }
        public static string SelectedFilePathData { get; set; }
        public static string SelectedFileNameData { get; set; }
        public static string FilesPathMod { get; set; }
        public static string SelectedFilePathMod { get; set; }
        public static string SelectedFileNameMod { get; set; }

        public static readonly string output = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output",
                                      outputHeightmap = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Heightmap",
                                      outputiTexture = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\iTexture",
                                      outputDDS = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\DDS",
                                      outputXML = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\XML",
                                      outputVideo = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Video",
                                      outputMods = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Mods",
                                      outputSwfMovie = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\SwfMovie",
                                      scripts = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts",
                                      scriptArchive = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\fbrb.pyw",
                                      scriptDBX = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\dbx.pyw",
                                      scriptSwfMovie = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\swfmovie.pyw",
                                      syntaxXML = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\syntaxXML.config",
                                      syntaxINI = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\syntaxINI.config",
                                      configGames = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\games.config",
                                      configSettings = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\settings.config",
                                      projects = Environment.CurrentDirectory + @"\BFBC2Toolkit\Projects",
                                      games = Environment.CurrentDirectory + @"\BFBC2Toolkit\Games",
                                      templateMod = Environment.CurrentDirectory + @"\BFBC2Toolkit\Templates\Mod",
                                      errorLog = Environment.CurrentDirectory + @"\BFBC2Toolkit\Logs\error.log",
                                      logs = Environment.CurrentDirectory + @"\BFBC2Toolkit\Logs";
    }
}
