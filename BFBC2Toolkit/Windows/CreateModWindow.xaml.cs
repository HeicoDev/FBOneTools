using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.FileIO;
using MahApps.Metro.Controls;
using BFBC2Toolkit.Data;
using BFBC2Toolkit.Functions;
using BFBC2Shared.Functions;

namespace BFBC2Toolkit
{
    public partial class CreateModWindow : MetroWindow
    {
        public CreateModWindow()
        {
            InitializeComponent();
        }

        private void CreateModWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnCreateMod.IsEnabled = false;
        }

        private async void BtnCreateMod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string projectPath = Dirs.Projects + @"\" + txtBoxName.Text;

                if (Directory.Exists(projectPath))
                {
                    var result = MessageBox.Show("A mod with the same name exists already.\nDo you want to overwrite the existing mod?", "Overwrite existing mod?", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                        return;
                    else if (result == MessageBoxResult.Yes)
                        await Task.Run(() => Directory.Delete(projectPath, true));
                }

                await Task.Run(() => FileSystem.CopyDirectory(Dirs.TemplateMod, projectPath));

                var iniFile = new IniFile(projectPath + @"\ModInfo.ini");
                iniFile.Write("Name", " " + txtBoxName.Text, "ModInfo");
                iniFile.Write("Summary", " " + txtBoxSummary.Text, "ModInfo");
                iniFile.Write("Author", " " + txtBoxAuthor.Text, "ModInfo");
                iniFile.Write("Version", " " + txtBoxVersion.Text, "ModInfo");
                iniFile.Write("Image", " " + txtBoxImage.Text, "ModInfo");
                iniFile.Write("Link", " " + txtBoxLink.Text, "ModInfo");

                Dirs.ModName = iniFile.Read("Name", "ModInfo");
                Dirs.FilesPathMod = projectPath;

                Globals.IsModAvailable = true;

                Tree.Populate(UIElements.TreeViewModExplorer, projectPath);

                Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to create mod! See error.log", "error");

                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TxtBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCreateModButton();
        }

        private void TxtBoxSummary_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCreateModButton();
        }

        private void TxtBoxAuthor_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCreateModButton();
        }

        private void TxtBoxVersion_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCreateModButton();
        }

        private void EnableCreateModButton()
        {
            if (txtBoxName.Text.Length > 2 && txtBoxSummary.Text.Length > 2 && txtBoxAuthor.Text.Length > 2 && txtBoxVersion.Text.Length > 0)
                if (txtBoxName.Text.StartsWith(" ") || txtBoxSummary.Text.StartsWith(" ") || txtBoxAuthor.Text.StartsWith(" ") || txtBoxVersion.Text.StartsWith(" "))
                    btnCreateMod.IsEnabled = false;
                else
                    btnCreateMod.IsEnabled = true;
            else
                btnCreateMod.IsEnabled = false;
        }
    }
}
