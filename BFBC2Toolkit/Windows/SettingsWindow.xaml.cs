﻿using System;
using System.Windows;
using MahApps.Metro.Controls;
using BFBC2Toolkit.Data;
using BFBC2Toolkit.Functions;
using BFBC2Shared.Data;
using BFBC2Shared.Functions;

namespace BFBC2Toolkit.Windows
{
    public partial class SettingsWindow : MetroWindow
    {
        private static bool hasSaved = false;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtBoxPathToPython.Text = SharedSettings.PathToPython;

            chkBoxHighlightSyntax.IsChecked = Settings.TxtEdHighlightSyntax;
            chkBoxHighlightCurrentLine.IsChecked = Settings.TxtEdHighlightCurrentLine;
            chkBoxShowLineNumbers.IsChecked = Settings.TxtEdShowLineNumbers;
            chkBoxClickableHyperlinks.IsChecked = Settings.TxtEdClickableHyperlinks;
            chkBoxHideCursorWhileTyping.IsChecked = Settings.TxtEdHideCursorWhileTyping;
            chkBoxShowTabs.IsChecked = Settings.TxtEdShowTabs;
            chkBoxCodeFolding.IsChecked = Settings.TxtEdCodeFolding;
            chkBoxCodeCompletion.IsChecked = Settings.TxtEdCodeCompletion;
            chkBoxShowEventLog.IsChecked = Settings.ShowEventLog;
            chkBoxShowProperties.IsChecked = Settings.ShowProperties;
            chkBoxShowArchiveFbrbPrompt.IsChecked = Settings.ShowArchiveFbrbPrompt;
            chkBoxAutoCheckUpdates.IsChecked = Settings.IsAutoUpdateCheckEnabled;           
        }

        private void ChkBoxHighlightSyntax_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHighlightSyntax = true;
        }

        private void ChkBoxHighlightSyntax_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHighlightSyntax = false;
        }

        private void ChkBoxHighlightCurrentLine_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHighlightCurrentLine = true;
        }

        private void ChkBoxHighlightCurrentLine_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHighlightCurrentLine = false;
        }        

        private void ChkBoxShowLineNumbers_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdShowLineNumbers = true;
        }

        private void ChkBoxShowLineNumbers_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdShowLineNumbers = false;
        }

        private void ChkBoxClickableHyperlinks_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdClickableHyperlinks = true;
        }

        private void ChkBoxClickableHyperlinks_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdClickableHyperlinks = false;
        }

        private void ChkBoxHideCursorWhileTyping_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHideCursorWhileTyping = true;
        }

        private void ChkBoxHideCursorWhileTyping_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHideCursorWhileTyping = false;
        }

        private void ChkBoxShowTabs_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdShowTabs = true;
        }

        private void ChkBoxShowTabs_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdShowTabs = false;
        }

        private void ChkBoxCodeFolding_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdCodeFolding = true;
        }

        private void ChkBoxCodeFolding_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdCodeFolding = false;
        }

        private void ChkBoxCodeCompletion_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdCodeCompletion = true;
        }

        private void ChkBoxCodeCompletion_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdCodeCompletion = false;
        }

        private void ChkBoxShowEventLog_Checked(object sender, RoutedEventArgs e)
        {
            Settings.ShowEventLog = true;
        }

        private void ChkBoxShowEventLog_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.ShowEventLog= false;
        }

        private void ChkBoxShowProperties_Checked(object sender, RoutedEventArgs e)
        {
            Settings.ShowProperties = true;
        }

        private void ChkBoxShowProperties_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.ShowProperties= false;
        }

        private void chkBoxShowArchiveFbrbPrompt_Checked(object sender, RoutedEventArgs e)
        {
            Settings.ShowArchiveFbrbPrompt = true;
        }

        private void chkBoxShowArchiveFbrbPrompt_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.ShowArchiveFbrbPrompt= false;
        }

        private void ChkBoxAutoCheckUpdates_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IsAutoUpdateCheckEnabled = true;
        }

        private void ChkBoxAutoCheckUpdates_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.IsAutoUpdateCheckEnabled = false;
        }

        private void BtnSelectPathToPython_Click(object sender, RoutedEventArgs e)
        {
            string pathOld = SharedSettings.PathToPython;

            string path = Python.ChangePath();

            if (path == String.Empty)
            {
                MessageBox.Show("Unable to locate pythonw.exe!", "Error");
            }
            else
            {
                bool isCorrectPythonVersion = Python.CheckVersion();

                if (isCorrectPythonVersion)
                {
                    txtBoxPathToPython.Text = path;
                }
                else
                {
                    SharedSettings.PathToPython = pathOld;

                    MessageBox.Show("Incorrect version of Python detected!\nIt must be version 2.7!", "Error");
                }
            }
        }

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Checking for updates...");

            Check.Update();

            Log.Write("", "done");
        }

        private void BtnSaveClose_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            hasSaved = true;
            
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!hasSaved)
                SettingsHandler.Load();
        }

        private void SaveSettings()
        {
            if (!Settings.TxtEdHighlightSyntax)
                UIElements.TextEditor.SyntaxHighlighting = null;

            UIElements.TextEditor.Options.HighlightCurrentLine = Settings.TxtEdHighlightCurrentLine;
            UIElements.TextEditor.ShowLineNumbers = Settings.TxtEdShowLineNumbers;
            UIElements.TextEditor.Options.EnableHyperlinks = Settings.TxtEdClickableHyperlinks;
            UIElements.TextEditor.Options.HideCursorWhileTyping = Settings.TxtEdHideCursorWhileTyping;
            UIElements.TextEditor.Options.ShowTabs = Settings.TxtEdShowTabs;  
            
            if (Settings.ShowEventLog)
            {
                UIElements.GridPreviewLogProp.RowDefinitions[2].MinHeight = 80;
                UIElements.GridPreviewLogProp.RowDefinitions[2].Height = new GridLength(121, GridUnitType.Star);
            }
            else
            {
                UIElements.GridPreviewLogProp.RowDefinitions[2].MinHeight = 0;
                UIElements.GridPreviewLogProp.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Star);
            }

            if (Settings.ShowProperties)
            {
                UIElements.GridPreviewProp.ColumnDefinitions[2].MinWidth = 100;
                UIElements.GridPreviewProp.ColumnDefinitions[2].Width = new GridLength(182, GridUnitType.Star);
            }
            else
            {
                UIElements.GridPreviewProp.ColumnDefinitions[2].MinWidth = 0;
                UIElements.GridPreviewProp.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
            }

            SettingsHandler.Save();
        }      
    }
}
