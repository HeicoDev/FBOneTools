using System;
using System.IO;
using System.Windows;
using System.Xml;
using Microsoft.Win32;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Data
{
    public class Settings
    {
        public static bool TxtEdHighlightSyntax { get; set; } = true;
        public static bool TxtEdHighlightCurrentLine { get; set; } = false;
        public static bool TxtEdShowLineNumbers { get; set; } = true;
        public static bool TxtEdClickableHyperlinks { get; set; } = true;
        public static bool TxtEdHideCursorWhileTyping { get; set; } = true;
        public static bool TxtEdShowTabs { get; set; } = false;
        public static bool TxtEdCodeFolding { get; set; } = true;
        public static bool TxtEdCodeCompletion { get; set; } = true;

        public static string PathToPython { get; set; } = @"C:\Python27\pythonw.exe";
    }

    public class SettingsHandler
    {
        public static void Save()
        {
            try
            {
                var xmlDocSettings = new XmlDocument();
                xmlDocSettings.Load(Dirs.ConfigSettings);
                var nodeList = xmlDocSettings.SelectNodes("/Settings/Setting");

                for (int i = 0; i < nodeList.Count; i++)
                {
                    switch (nodeList[i].Attributes["Name"].Value)
                    {
                        case "TxtEdHighlightSyntax":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdHighlightSyntax.ToString();
                            break;
                        case "TxtEdHighlightCurrentLine":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdHighlightCurrentLine.ToString();
                            break;
                        case "TxtEdShowLineNumbers":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdShowLineNumbers.ToString();
                            break;
                        case "TxtEdClickableHyperlinks":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdClickableHyperlinks.ToString();
                            break;
                        case "TxtEdHideCursorWhileTyping":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdHideCursorWhileTyping.ToString();
                            break;
                        case "TxtEdShowTabs":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdShowTabs.ToString();
                            break;
                        case "TxtEdCodeFolding":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdCodeFolding.ToString();
                            break;
                        case "TxtEdCodeCompletion":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtEdCodeCompletion.ToString();
                            break;
                        case "PathToPython":
                            nodeList[i].Attributes["Value"].Value = Settings.PathToPython;
                            break;
                    }
                }

                xmlDocSettings.Save(Dirs.ConfigSettings);
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to save settings! See error.log", "Error");
            }
        }

        public static void Load()
        {
            try
            {
                var xmlDocSettings = new XmlDocument();
                xmlDocSettings.Load(Dirs.ConfigSettings);
                var nodeList = xmlDocSettings.SelectNodes("/Settings/Setting");

                for (int i = 0; i < nodeList.Count; i++)
                {
                    switch (nodeList[i].Attributes["Name"].Value)
                    {
                        case "TxtEdHighlightSyntax":
                            Settings.TxtEdHighlightSyntax = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "TxtEdHighlightCurrentLine":
                            Settings.TxtEdHighlightCurrentLine = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "TxtEdShowLineNumbers":
                            Settings.TxtEdShowLineNumbers = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "TxtEdClickableHyperlinks":
                            Settings.TxtEdClickableHyperlinks = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "TxtEdHideCursorWhileTyping":
                            Settings.TxtEdHideCursorWhileTyping = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "TxtEdShowTabs":
                            Settings.TxtEdShowTabs = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "TxtEdCodeFolding":
                            Settings.TxtEdCodeFolding = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "TxtEdCodeCompletion":
                            Settings.TxtEdCodeCompletion = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "PathToPython":
                            Settings.PathToPython = nodeList[i].Attributes["Value"].Value;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                MessageBox.Show("Unable to load settings! See error.log", "Error");
            }
        }

        public static string ChangePythonPath()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "exe file (.exe)|*.exe";
            ofd.Title = "Select pythonw.exe...";

            if (ofd.ShowDialog() == true)
            {
                string path = ofd.FileName;

                if (path.EndsWith("pythonw.exe"))
                {
                    Settings.PathToPython = path;

                    return path;
                }
                else
                {
                    path = Path.GetDirectoryName(path) + @"\pythonw.exe";

                    if (File.Exists(path))
                    {                       
                        Settings.PathToPython = path;

                        return path;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            return String.Empty;
        }
    }
}
