using System.Windows;
using MahApps.Metro.Controls;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Windows
{
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.TxtEdHighlightCurrentLine)
                chkBoxHighlightCurrentLine.IsChecked = true;
        }

        private void ChkBoxHighlightCurrentLine_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHighlightCurrentLine = true;
        }

        private void ChkBoxHighlightCurrentLine_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.TxtEdHighlightCurrentLine = false;
        }       

        private void BtnSaveClose_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.TxtEdHighlightCurrentLine)
                Elements.TextEditor.Options.HighlightCurrentLine = true;
            else
                Elements.TextEditor.Options.HighlightCurrentLine = false;

            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
