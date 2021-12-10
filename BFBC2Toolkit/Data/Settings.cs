using System;
using System.Windows;
using System.Xml;
using BFBC2Shared.Functions;
using BFBC2Shared.Data;

namespace BFBC2Toolkit.Data
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
        public static bool IsAutoUpdateCheckEnabled { get; set; } = true;
    }

    public class SettingsHandler
    {
        public static bool Save()
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
                            nodeList[i].Attributes["Value"].Value = SharedSettings.PathToPython;
                            break;
                        case "IsAutoUpdateCheckEnabled":
                            nodeList[i].Attributes["Value"].Value = Settings.IsAutoUpdateCheckEnabled.ToString();
                            break;
                    }
                }

                xmlDocSettings.Save(Dirs.ConfigSettings);

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                MessageBox.Show("Unable to save settings! See error.log", "error");

                return true;
            }
        }

        public static bool Load()
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
                            SharedSettings.PathToPython = nodeList[i].Attributes["Value"].Value;
                            break;
                        case "IsAutoUpdateCheckEnabled":
                            Settings.IsAutoUpdateCheckEnabled = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                MessageBox.Show("Unable to load settings! See error.log", "error");

                return true;
            }
        }
    }
}
