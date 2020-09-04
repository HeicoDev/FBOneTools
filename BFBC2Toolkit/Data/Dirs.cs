using System;

namespace BFBC2Toolkit.Data
{
    public class Dirs
    {
        /// <summary>
        /// Temp Directories
        /// </summary>
        
        public static string ModName { get; set; }
        public static string FilePath { get; set; }
        public static string FilesPathData { get; set; }
        public static string SelectedFilePathData { get; set; }
        public static string SelectedFileNameData { get; set; }
        public static string FilesPathMod { get; set; }
        public static string SelectedFilePathMod { get; set; }
        public static string SelectedFileNameMod { get; set; }

        /// <summary>
        /// Persistent Directories
        /// </summary>
        
        public static string Output { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output";
        public static string OutputHeightmap { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Heightmap";
        public static string OutputiTexture { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\iTexture";
        public static string OutputDDS { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\DDS";
        public static string OutputXML { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\XML";
        public static string OutputVideo { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Video";
        public static string OutputMods { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\Mods";
        public static string OutputSwfMovie { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Output\SwfMovie";
        public static string Scripts { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts";
        public static string ScriptArchive { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\fbrb.pyw";
        public static string ScriptDBX { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\dbx.pyw";
        public static string ScriptSwfMovie { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Scripts\swfmovie.pyw";
        public static string SyntaxXML { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\SyntaxXML.Config";
        public static string SyntaxINI { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\SyntaxINI.Config";
        public static string ConfigGames { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\games.Config";
        public static string ConfigSettings { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Config\settings.Config";
        public static string Logs { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Logs";
        public static string ErrorLog { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Logs\error.log";
        public static string Projects { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Projects";
        public static string Games { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Games";
        public static string TemplateMod { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Templates\Mod";       
        public static string Docs { get; } = Environment.CurrentDirectory + @"\BFBC2Toolkit\Docs";
    }
}
