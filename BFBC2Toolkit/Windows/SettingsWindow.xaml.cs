using System;
using System.Windows;
using MahApps.Metro.Controls;
using BFBC2Toolkit.Data;

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
            txtBoxPathToPython.Text = Settings.PathToPython;

            chkBoxHighlightSyntax.IsChecked = Settings.TxtEdHighlightSyntax;
            chkBoxHighlightCurrentLine.IsChecked = Settings.TxtEdHighlightCurrentLine;
            chkBoxShowLineNumbers.IsChecked = Settings.TxtEdShowLineNumbers;
            chkBoxClickableHyperlinks.IsChecked = Settings.TxtEdClickableHyperlinks;
            chkBoxHideCursorWhileTyping.IsChecked = Settings.TxtEdHideCursorWhileTyping;
            chkBoxShowTabs.IsChecked = Settings.TxtEdShowTabs;
            chkBoxCodeFolding.IsChecked = Settings.TxtEdCodeFolding;
            chkBoxCodeCompletion.IsChecked = Settings.TxtEdCodeCompletion;
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

        private void BtnSelectPathToPython_Click(object sender, RoutedEventArgs e)
        {
            string path = SettingsHandler.ChangePythonPath();

            if (path == String.Empty)
                MessageBox.Show("Unable to locate pythonw.exe!", "Error");            
            else
                txtBoxPathToPython.Text = path;
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

            SettingsHandler.Save();
        }        
    }
}
