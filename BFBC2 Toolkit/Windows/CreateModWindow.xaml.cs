using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualBasic.FileIO;
using MahApps.Metro.Controls;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit
{
    public partial class CreateModWindow : MetroWindow
    {
        private TreeView treeViewModExplorer;

        public CreateModWindow(TreeView treeView)
        {
            InitializeComponent();

            treeViewModExplorer = treeView;
        }

        private void CreateModWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnCreateMod.IsEnabled = false;
        }

        private async void BtnCreateMod_Click(object sender, RoutedEventArgs e)
        {
            string projectPath = Dirs.projects + @"\" + txtBoxName.Text;

            if (Directory.Exists(projectPath))
            {
                var result = MessageBox.Show("A mod with the same name exists already.\nDo you want to overwrite the existing mod?", "Overwrite existing mod?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                    return;
                else if (result == MessageBoxResult.Yes)
                    await Task.Run(() => Directory.Delete(projectPath, true));
            }

            await Task.Run(() => FileSystem.CopyDirectory(Dirs.templateMod, projectPath));

            var iniFile = new IniFile(projectPath + @"\ModInfo.ini");
            iniFile.Write("Name", " " + txtBoxName.Text, "ModInfo");
            iniFile.Write("Summary", " " + txtBoxSummary.Text, "ModInfo");
            iniFile.Write("Author", " " + txtBoxAuthor.Text, "ModInfo");
            iniFile.Write("Version", " " + txtBoxVersion.Text, "ModInfo");
            iniFile.Write("Image", " " + txtBoxImage.Text, "ModInfo");
            iniFile.Write("Link", " " + txtBoxLink.Text, "ModInfo");

            Dirs.modName = iniFile.Read("Name", "ModInfo");
            Dirs.filesPathMod = projectPath;

            Vars.isModAvailable = true;

            Tree.Populate(treeViewModExplorer, projectPath);

            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCreateModButton();
        }

        private void txtBoxSummary_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCreateModButton();
        }

        private void txtBoxAuthor_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCreateModButton();
        }

        private void txtBoxVersion_TextChanged(object sender, TextChangedEventArgs e)
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
