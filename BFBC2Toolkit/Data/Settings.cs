﻿using System;
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
        public static double TxtFontSize { get; set; } = 11;
        public static string PathToPython { get; set; } = @"C:\Python27\pythonw.exe";
        public static bool ShowEventLog { get; set; } = true;
        public static bool ShowProperties { get; set; } = true;
        public static bool ShowArchiveFbrbPrompt { get; set; } = true;
        public static bool IsAutoUpdateCheckEnabled { get; set; } = true;
        public static string MainWindowState { get; set; } = "Normal";
        public static double MainWindowWidth { get; set; } = 800;
        public static double MainWindowHeight { get; set; } = 450;
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
                        case "TxtFontSize":
                            nodeList[i].Attributes["Value"].Value = Settings.TxtFontSize.ToString();
                            break;
                        case "PathToPython":
                            nodeList[i].Attributes["Value"].Value = SharedSettings.PathToPython;
                            break;
                        case "ShowEventLog":
                            nodeList[i].Attributes["Value"].Value = Settings.ShowEventLog.ToString();
                            break;
                        case "ShowProperties":
                            nodeList[i].Attributes["Value"].Value = Settings.ShowProperties.ToString();
                            break;
                        case "ShowArchiveFbrbPrompt":
                            nodeList[i].Attributes["Value"].Value = Settings.ShowArchiveFbrbPrompt.ToString();
                            break;
                        case "IsAutoUpdateCheckEnabled":
                            nodeList[i].Attributes["Value"].Value = Settings.IsAutoUpdateCheckEnabled.ToString();
                            break;
                        case "MainWindowState":
                            nodeList[i].Attributes["Value"].Value = Settings.MainWindowState;
                            break;
                        case "MainWindowWidth":
                            nodeList[i].Attributes["Value"].Value = Settings.MainWindowWidth.ToString();
                            break;
                        case "MainWindowHeight":
                            nodeList[i].Attributes["Value"].Value = Settings.MainWindowHeight.ToString();
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
                        case "TxtFontSize":
                            Settings.TxtFontSize = Convert.ToDouble(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "PathToPython":
                            SharedSettings.PathToPython = nodeList[i].Attributes["Value"].Value;
                            break;
                        case "ShowEventLog":
                            Settings.ShowEventLog = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "ShowProperties":
                            Settings.ShowProperties = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "ShowArchiveFbrbPrompt":
                            Settings.ShowArchiveFbrbPrompt = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "IsAutoUpdateCheckEnabled":
                            Settings.IsAutoUpdateCheckEnabled = Convert.ToBoolean(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "MainWindowState":
                            Settings.MainWindowState = nodeList[i].Attributes["Value"].Value;
                            break;
                        case "MainWindowWidth": 
                            Settings.MainWindowWidth = Convert.ToDouble(nodeList[i].Attributes["Value"].Value);
                            break;
                        case "MainWindowHeight":
                            Settings.MainWindowHeight = Convert.ToDouble(nodeList[i].Attributes["Value"].Value);
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
