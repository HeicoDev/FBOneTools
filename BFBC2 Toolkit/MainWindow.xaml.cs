using System;
using System.IO;
using System.IO.Compression;
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
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO;
using MahApps.Metro.Controls;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Functions;
using BFBC2_Toolkit.Tools;
using BFBC2_Toolkit.Windows;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

/// <summary>
/// BFBC2 Toolkit 
/// By Nico Hellmund 
/// Aka Heico
/// Copyright 2020
/// </summary>

namespace BFBC2_Toolkit
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeStartup();
        }

        private async void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (textEditor.Visibility == Visibility.Visible)
            {
                if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.KeyboardDevice.IsKeyDown(Key.F))
                    FindAndReplaceWindow.ShowForReplace(textEditor);

                if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.KeyboardDevice.IsKeyDown(Key.S))
                    await SaveTextEditor();
            }
        }

        private async void BtnAddGame_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "exe file (.exe)|*.exe";
            ofd.Title = "Select game or server executable...";

            if (ofd.ShowDialog() == true)
            {
                await Profile.Add(ofd);
            }

            /*
            var xmlDoc = XDocument.Load(Dirs.configGames);
            var games = xmlDoc.Element("Games");
            var game = new XElement("Game");
            game.Add(new XAttribute("Name", "Battlefield Test"));
            game.Add(new XAttribute("Platform", "PC"));
            games.Add(game);
            xmlDoc.Save(Dirs.configGames);
            */

            //Vars.isGameProfile = true;
        }

        private void BtnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            SelectGameWindow selectGameWindow = new SelectGameWindow(treeViewDataExplorer);
            selectGameWindow.Owner = this;
            selectGameWindow.ShowDialog();

            if (Vars.isGameProfile)
                btnArchiveFbrb.IsEnabled = false;
        }

        private async void BtnCreateMod_Click(object sender, RoutedEventArgs e)
        {
            await DisposeMediaStream();

            var createModWindow = new CreateModWindow(treeViewModExplorer);
            createModWindow.Owner = this;
            createModWindow.ShowDialog();

            if (treeViewModExplorer.HasItems == true)
                btnArchiveMod.IsEnabled = true;
        }

        private async void BtnOpenMod_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "ini file (.ini)|*.ini";
            ofd.Title = "Select ModInfo.ini...";
            ofd.InitialDirectory = Dirs.projects;

            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileName.Contains("ModInfo.ini"))
                {
                    Write.ToEventLog("Loading mod files...", "");

                    await DisposeMediaStream();

                    Dirs.filesPathMod = ofd.FileName.Replace(@"\ModInfo.ini", "");

                    var iniFile = new IniFile(ofd.FileName);

                    Dirs.modName = iniFile.Read("Name", "ModInfo");

                    Tree.Populate(treeViewModExplorer, Dirs.filesPathMod);

                    Vars.isModAvailable = true;

                    btnArchiveMod.IsEnabled = true;

                    Write.ToEventLog("", "done");
                }
            }
        }

        private async void BtnExtractMod_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "zip file (.zip)|*.zip";
            ofd.Title = "Select zip archive...";

            if (ofd.ShowDialog() == true)
            {
                Write.ToEventLog("Extracting mod archive...", "");

                await DisposeMediaStream();

                using (var fileStream = new FileStream(ofd.FileName, FileMode.Open))
                {
                    using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                    {
                        var entry = await Task.Run(() => zipArchive.GetEntry("ModInfo.ini"));

                        if (entry != null)
                        {
                            string tempFile = Path.GetTempFileName();

                            await Task.Run(() => entry.ExtractToFile(tempFile, true));

                            var iniFile = new IniFile(tempFile);

                            string name = iniFile.Read("Name", "ModInfo"),
                                   path = Dirs.projects + @"\" + name;

                            if (Directory.Exists(path))
                            {
                                var result = MessageBox.Show("A mod with the same name exists already.\nThe old mod must be overwritten to continue.\nDo you want to continue?", "Overwrite existing mod?", MessageBoxButton.YesNo);

                                if (result == MessageBoxResult.Yes)
                                {
                                    await Task.Run(() => Directory.Delete(path, true));

                                    await Task.Run(() => zipArchive.ExtractToDirectory(path));
                                }
                                else
                                {
                                    if (File.Exists(tempFile))
                                        await Task.Run(() => File.Delete(tempFile));

                                    Write.ToEventLog("Extraction aborted.", "");

                                    return;
                                }
                            }
                            else
                            {
                                if (Directory.Exists(path))
                                    await Task.Run(() => Directory.Delete(path, true));

                                await Task.Run(() => zipArchive.ExtractToDirectory(path));
                            }

                            if (File.Exists(tempFile))
                                await Task.Run(() => File.Delete(tempFile));

                            Tree.Populate(treeViewModExplorer, path);

                            Dirs.filesPathMod = path;
                            Dirs.modName = name;

                            Vars.isModAvailable = true;

                            btnArchiveMod.IsEnabled = true;

                            Write.ToEventLog("", "done");
                        }
                    }
                }
            }
        }

        private async void BtnArchiveMod_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Archiving mod...", "");

            await DisposeMediaStream();

            string zipFile = Dirs.outputMods + @"\" + Dirs.modName + ".zip",
                   projectPath = Dirs.projects + @"\" + Dirs.modName;

            if (File.Exists(zipFile))
                await Task.Run(() => File.Delete(zipFile));

            Write.ToEventLog("Cleaning up files...", "");

            await Task.Run(() => CleanUp.FilesAndDirs(projectPath));

            await Task.Run(() => ZipFile.CreateFromDirectory(projectPath, zipFile));

            Write.ToEventLog("", "done");
        }

        private async void BtnExtractFbrb_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "fbrb file (.fbrb)|*.fbrb";
            ofd.Title = "Select fbrb archive...";

            if (ofd.ShowDialog() == true)
            {
                Write.ToEventLog("This may take a while. Extracting fbrb archive, please wait...", "");

                await DisposeMediaStream();

                if (Directory.Exists(Dirs.filesPathData) && Vars.isGameProfile == false)
                    await Task.Run(() => Directory.Delete(Dirs.filesPathData, true));

                var process = Process.Start(Dirs.scriptArchive, "\"" + ofd.FileName.Replace(@"\", @"\\"));
                await Task.Run(() => process.WaitForExit());

                Dirs.filesPathData = ofd.FileName.Replace(".fbrb", " FbRB");

                Write.ToEventLog("Cleaning up files, please wait...", "");

                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.filesPathData));

                Tree.Populate(treeViewDataExplorer, Dirs.filesPathData);

                Vars.isGameProfile = false;

                btnArchiveFbrb.IsEnabled = true;

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnArchiveFbrb_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("This may take a while. Archiving fbrb archive, please wait...", "");

            await DisposeMediaStream();

            var process = Process.Start(Dirs.scriptArchive, "\"" + Dirs.filesPathData.Replace(@"\", @"\\"));
            await Task.Run(() => process.WaitForExit());

            Write.ToEventLog("", "done");
        }

        private async void BtnCopyToMod_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.isDataTreeView == true && Vars.isModAvailable == true && treeViewDataExplorer.SelectedItem != null)
            {
                Write.ToEventLog("Copying file...", "");

                await DisposeMediaStream();

                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.filesPathMod));

                string filePathMod = Dirs.filesPathMod + @"\" + Dirs.filePath;

                foreach (string gameId in Vars.gameIds)
                {
                    if (filePathMod.Contains(gameId))
                    {
                        filePathMod = filePathMod.Replace(gameId + @"\", "");
                        break;
                    }
                }

                if (File.Exists(filePathMod))
                    await Task.Run(() => File.Delete(filePathMod));

                var fileInfo = new FileInfo(filePathMod);
                await Task.Run(() => fileInfo.Directory.Create());

                await Task.Run(() => File.Copy(Dirs.selectedFilePathData, filePathMod));

                Tree.Populate(treeViewModExplorer, Dirs.filesPathMod);

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Exporting file...", "");

            string selectedFilePath = "",
                   selectedFileName = "";

            if (Vars.isDataTreeView == true)
            {
                selectedFilePath = Dirs.selectedFilePathData;
                selectedFileName = Dirs.selectedFileNameData;
            }
            else
            {
                selectedFilePath = Dirs.selectedFilePathMod;
                selectedFileName = Dirs.selectedFileNameMod;
            }

            if (selectedFilePath.EndsWith(".dbx"))
            {
                string path = selectedFilePath.Replace(".dbx", ".xml"),
                       name = selectedFileName.Replace(".dbx", ".xml");

                if (File.Exists(Dirs.outputXML + @"\" + name))
                    await Task.Run(() => File.Delete(Dirs.outputXML + @"\" + name));

                await Task.Run(() => File.Copy(path, Dirs.outputXML + @"\" + name));
            }
            else if (selectedFilePath.EndsWith(".itexture"))
            {
                string path = selectedFilePath.Replace(".itexture", ".dds"),
                       name = selectedFileName.Replace(".itexture", ".dds");

                if (File.Exists(Dirs.outputDDS + @"\" + name))
                    await Task.Run(() => File.Delete(Dirs.outputDDS + @"\" + name));

                await Task.Run(() => File.Copy(path, Dirs.outputDDS + @"\" + name));
            }
            else if (selectedFilePath.EndsWith(".ps3texture"))
            {
                string path = selectedFilePath.Replace(".ps3texture", ".dds"),
                       name = selectedFileName.Replace(".ps3texture", ".dds");

                if (File.Exists(Dirs.outputDDS + @"\" + name))
                    await Task.Run(() => File.Delete(Dirs.outputDDS + @"\" + name));

                await Task.Run(() => File.Copy(path, Dirs.outputDDS + @"\" + name));
            }
            else if (selectedFilePath.EndsWith(".terrainheightfield"))
            {
                string path = selectedFilePath.Replace(".terrainheightfield", ".raw"),
                       name = selectedFileName.Replace(".terrainheightfield", ".raw");

                if (File.Exists(Dirs.outputHeightmap + @"\" + name))
                    await Task.Run(() => File.Delete(Dirs.outputHeightmap + @"\" + name));

                await Task.Run(() => File.Copy(path, Dirs.outputHeightmap + @"\" + name));
            }
            else if (selectedFilePath.EndsWith(".binkmemory"))
            {
                string name = selectedFileName.Replace(".binkmemory", ".bik");

                if (File.Exists(Dirs.outputVideo + @"\" + name))
                    await Task.Run(() => File.Delete(Dirs.outputVideo + @"\" + name));

                await Task.Run(() => File.Copy(selectedFilePath, Dirs.outputVideo + @"\" + name));
            }

            Write.ToEventLog("Exported file to output folder.", "done");
        }

        private async void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            await DisposeMediaStream();

            string selectedFilePath = "";

            if (Vars.isDataTreeView == true)
                selectedFilePath = Dirs.selectedFilePathData;
            else
                selectedFilePath = Dirs.selectedFilePathMod;

            if (selectedFilePath.EndsWith(".dbx"))
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "xml file (.xml)|*.xml";
                ofd.Title = "Select xml file...";

                if (ofd.ShowDialog() == true)
                {
                    Write.ToEventLog("Importing file...", "");

                    string path = selectedFilePath.Replace(".dbx", ".xml");

                    if (File.Exists(path))
                        await Task.Run(() => File.Delete(path));

                    await Task.Run(() => File.Copy(ofd.FileName, path));

                    var process = Process.Start(Dirs.scriptDBX, "\"" + path.Replace(@"\", @"\\"));
                    await Task.Run(() => process.WaitForExit());

                    textEditor.Text = await Task.Run(() => File.ReadAllText(path));
                    textEditor.ScrollToHome();

                    Write.ToEventLog("", "done");
                }
            }
            else if (selectedFilePath.EndsWith(".itexture"))
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "dds file (.dds)|*.dds";
                ofd.Title = "Select dds file...";

                if (ofd.ShowDialog() == true)
                {
                    Write.ToEventLog("Importing file...", "");

                    string path = selectedFilePath.Replace(".itexture", ".dds");

                    if (File.Exists(path))
                        await Task.Run(() => File.Delete(path));

                    await Task.Run(() => File.Copy(ofd.FileName, path));

                    string[] file = { path };

                    await Task.Run(() => TextureConverter.ConvertFile(file, false));

                    try
                    {
                        var bitmap = new BitmapImage();
                        mediaStream = new FileStream(path, FileMode.Open);

                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.None;
                        bitmap.StreamSource = mediaStream;
                        bitmap.EndInit();

                        bitmap.Freeze();
                        image.Source = bitmap;
                    }
                    catch
                    {
                        image.Visibility = Visibility.Hidden;

                        Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "error");
                    }

                    Write.ToEventLog("", "done");
                }
            }
            else if (selectedFilePath.EndsWith(".bik"))
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "bik file (.bik)|*.bik";
                ofd.Title = "Select bik file...";

                if (ofd.ShowDialog() == true)
                {
                    Write.ToEventLog("Importing file...", "");

                    await RenameToBik();

                    string path = selectedFilePath.Replace(".binkmemory", ".mp4");

                    if (File.Exists(path))
                        await Task.Run(() => File.Delete(path));

                    if (File.Exists(selectedFilePath.Replace(".binkmemory", ".bik")))
                        await Task.Run(() => File.Delete(selectedFilePath.Replace(".binkmemory", ".bik")));

                    await Task.Run(() => File.Copy(ofd.FileName, path));

                    if (File.Exists(selectedFilePath))
                        await Task.Run(() => File.Delete(selectedFilePath));

                    await Task.Run(() => File.Copy(ofd.FileName, selectedFilePath));

                    await PlayVideo(path);

                    Write.ToEventLog("", "done");
                }
            }
        }

        private void BtnOpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = "";

            if (Vars.isDataTreeView == true)
                selectedFilePath = Dirs.selectedFilePathData;
            else
                selectedFilePath = Dirs.selectedFilePathMod;

            Process.Start("explorer.exe", "/select, " + selectedFilePath);
        }

        private async void DataExplorer_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                Vars.isDataTreeView = true;

                if (treeViewModExplorer.SelectedItem != null)
                    (treeViewModExplorer.SelectedItem as CustomTreeViewItem).IsSelected = false;

                treeViewDataExplorer.Focus();

                if (Dirs.selectedFilePathData.EndsWith(".binkmemory"))
                    await RenameToBik();

                var tvi = treeViewDataExplorer.SelectedItem as CustomTreeViewItem;

                if (tvi != null)
                {
                    Dirs.selectedFileNameData = tvi.Name;
                    Dirs.selectedFilePathData = tvi.Path;

                    if (Dirs.selectedFilePathData.Contains(Dirs.filesPathData))
                        Dirs.filePath = Dirs.selectedFilePathData.Replace(Dirs.filesPathData, "");

                    /*
                    for (int i = 0; i < 1000; i++)
                    {
                        tvi = tvi.Parent as TreeViewItem;

                        if (tvi == null)
                            break;

                        header = tvi.Header.ToString();
                        path = header + @"\" + path;
                    }
                    */

                    /*
                    if (header.Length != 0)
                        path = path.Replace(header, "");
                    */                    

                    if (Dirs.selectedFileNameData.EndsWith(".dbx"))
                    {
                        await ChangeInterface("dbx");
                        Write.ToInfoBox(tvi);

                        if (!File.Exists(Dirs.selectedFilePathData.Replace(".dbx", ".xml")))
                        {
                            var process = Process.Start(Dirs.scriptDBX, "\"" + Dirs.selectedFilePathData);
                            await Task.Run(() => process.WaitForExit());
                        }

                        var reader = new XmlTextReader(Dirs.syntaxXML);
                        textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        reader.Close();

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathData.Replace(".dbx", ".xml")));
                        textEditor.ScrollToHome();
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".ini"))
                    {
                        await ChangeInterface("ini");
                        Write.ToInfoBox(tvi);

                        var reader = new XmlTextReader(Dirs.syntaxINI);
                        textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        reader.Close();

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathData));
                        textEditor.ScrollToHome();
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".txt"))
                    {
                        await ChangeInterface("txt");
                        Write.ToInfoBox(tvi);

                        textEditor.SyntaxHighlighting = null;

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathData));
                        textEditor.ScrollToHome();
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".itexture"))
                    {
                        await ChangeInterface("texture");

                        string[] file = { Dirs.selectedFilePathData };

                        //if (!File.Exists(Dirs.selectedFilePathData.Replace(".itexture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            mediaStream = new FileStream(Dirs.selectedFilePathData.Replace(".itexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = mediaStream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "error");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".ps3texture"))
                    {
                        await ChangeInterface("ps3texture");

                        string[] file = { Dirs.selectedFilePathData };

                        //if (!File.Exists(Dirs.selectedFilePathData.Replace(".ps3texture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            mediaStream = new FileStream(Dirs.selectedFilePathData.Replace(".ps3texture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = mediaStream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "error");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".xenontexture"))
                    {
                        await ChangeInterface("xenontexture");

                        string[] file = { Dirs.selectedFilePathData };

                        //if (!File.Exists(Dirs.selectedFilePathData.Replace(".ps3texture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            mediaStream = new FileStream(Dirs.selectedFilePathData.Replace(".xenontexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = mediaStream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "error");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".terrainheightfield"))
                    {
                        await ChangeInterface("heightmap");
                        Write.ToInfoBox(tvi);

                        string[] file = { Dirs.selectedFilePathData };

                        if (!File.Exists(Dirs.selectedFilePathData.Replace(".terrainheightfield", ".raw")))
                            await Task.Run(() => TextureConverter.ConvertFile(file, false));
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".binkmemory"))
                    {
                        await ChangeInterface("video");
                        Write.ToInfoBox(tvi);

                        string mp4 = Dirs.selectedFilePathData.Replace(".binkmemory", ".mp4"),
                              bik = Dirs.selectedFilePathData.Replace(".binkmemory", ".bik");

                        if (!File.Exists(bik))
                            await Task.Run(() => File.Copy(Dirs.selectedFilePathData, bik));

                        if (File.Exists(mp4))
                            await Task.Run(() => File.Delete(mp4));

                        await Task.Run(() => FileSystem.RenameFile(bik, Dirs.selectedFileNameData.Replace(".binkmemory", ".mp4")));

                        await PlayVideo(mp4);
                    }
                    else
                    {
                        await ChangeInterface("");
                        Write.ToInfoBox(tvi);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void ModExplorer_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                /*
                Vars.isDataTreeView = false;

                if (treeViewDataExplorer.SelectedItem != null)
                    (treeViewDataExplorer.SelectedItem as TreeViewItem).IsSelected = false;

                treeViewModExplorer.Focus();

                if (Dirs.selectedFilePathMod.EndsWith(".binkmemory"))
                    await RenameToBik();

                var tvi = treeViewModExplorer.SelectedItem as TreeViewItem;

                if (tvi != null)
                {
                    string path = tvi.Header.ToString(),
                           header = "";

                    Dirs.selectedFileNameMod = path;

                    for (int i = 0; i < 1000; i++)
                    {
                        tvi = tvi.Parent as TreeViewItem;

                        if (tvi == null)
                            break;

                        header = tvi.Header.ToString();
                        path = header + @"\" + path;
                    }

                    if (header.Length != 0)
                        path = path.Replace(header, "");

                    Dirs.filePath = path;

                    Dirs.selectedFilePathMod = Dirs.filesPathMod + path;

                    if (path.EndsWith(".dbx"))
                    {
                        await ChangeInterface("dbx");
                        Write.ToInfoBox(Dirs.selectedFileNameMod);

                        if (!File.Exists(Dirs.selectedFilePathMod.Replace(".dbx", ".xml")))
                        {
                            var process = Process.Start(Dirs.scriptDBX, "\"" + Dirs.filesPathMod + path);
                            await Task.Run(() => process.WaitForExit());
                        }

                        var reader = new XmlTextReader(Dirs.syntaxXML);
                        textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        reader.Close();

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathMod.Replace(".dbx", ".xml")));
                        textEditor.ScrollToHome();
                    }
                    else if (path.EndsWith(".ini"))
                    {
                        await ChangeInterface("ini");
                        Write.ToInfoBox(Dirs.selectedFileNameMod);

                        var reader = new XmlTextReader(Dirs.syntaxINI);
                        textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        reader.Close();

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.filesPathMod + path));
                        textEditor.ScrollToHome();
                    }
                    else if (path.EndsWith(".txt"))
                    {
                        await ChangeInterface("txt");
                        Write.ToInfoBox(Dirs.selectedFileNameMod);

                        textEditor.SyntaxHighlighting = null;

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.filesPathMod + path));
                        textEditor.ScrollToHome();
                    }
                    else if (path.EndsWith(".itexture"))
                    {
                        await ChangeInterface("texture");

                        string[] file = { Dirs.selectedFilePathMod };

                        //if (!File.Exists(Dirs.selectedFilePathMod.Replace(".itexture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            mediaStream = new FileStream(Dirs.selectedFilePathMod.Replace(".itexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = mediaStream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "error");
                        }

                        Write.ToInfoBox(Dirs.selectedFileNameData);
                    }
                    else if (path.EndsWith(".ps3texture"))
                    {
                        await ChangeInterface("ps3texture");

                        string[] file = { Dirs.selectedFilePathMod };

                        //if (!File.Exists(Dirs.selectedFilePathMod.Replace(".ps3texture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            mediaStream = new FileStream(Dirs.selectedFilePathMod.Replace(".ps3texture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = mediaStream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "error");
                        }

                        Write.ToInfoBox(Dirs.selectedFileNameData);
                    }
                    else if (path.EndsWith(".xenontexture"))
                    {
                        await ChangeInterface("xenontexture");

                        string[] file = { Dirs.selectedFilePathMod };

                        //if (!File.Exists(Dirs.selectedFilePathMod.Replace(".ps3texture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            mediaStream = new FileStream(Dirs.selectedFilePathMod.Replace(".xenontexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = mediaStream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "error");
                        }

                        Write.ToInfoBox(Dirs.selectedFileNameData);
                    }
                    else if (path.EndsWith(".terrainheightfield"))
                    {
                        await ChangeInterface("heightmap");
                        Write.ToInfoBox(Dirs.selectedFileNameMod);

                        string[] file = { Dirs.selectedFilePathMod };

                        if (!File.Exists(Dirs.selectedFilePathMod.Replace(".terrainheightfield", ".raw")))
                            await Task.Run(() => TextureConverter.ConvertFile(file, false));
                    }
                    else if (path.EndsWith(".binkmemory"))
                    {
                        await ChangeInterface("video");
                        Write.ToInfoBox(Dirs.selectedFileNameMod);

                        string mp4 = Dirs.selectedFilePathMod.Replace(".binkmemory", ".mp4"),
                               bik = Dirs.selectedFilePathMod.Replace(".binkmemory", ".bik");

                        if (!File.Exists(bik))
                            await Task.Run(() => File.Copy(Dirs.selectedFilePathMod, bik));

                        if (File.Exists(mp4))
                            await Task.Run(() => File.Delete(mp4));

                        await Task.Run(() => FileSystem.RenameFile(bik, Dirs.selectedFileNameMod.Replace(".binkmemory", ".mp4")));

                        await PlayVideo(mp4);
                    }
                    else
                    {
                        await ChangeInterface("");
                        Write.ToInfoBox(Dirs.selectedFileNameMod);
                    }
                }
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            await SaveTextEditor();
        }

        private async Task SaveTextEditor()
        {
            Write.ToEventLog("Saving file...", "");

            string selectedFilePath = "";

            if (Vars.isDataTreeView == true)
                selectedFilePath = Dirs.selectedFilePathData;
            else
                selectedFilePath = Dirs.selectedFilePathMod;

            string textEditorText = textEditor.Text;

            if (selectedFilePath.EndsWith(".dbx"))
            {
                string path = selectedFilePath.Replace(".dbx", ".xml");

                await Task.Run(() => File.WriteAllText(path, textEditorText));

                var process = Process.Start(Dirs.scriptDBX, "\"" + path.Replace(@"\", @"\\"));
                await Task.Run(() => process.WaitForExit());
            }
            else if (selectedFilePath.EndsWith(".ini") || selectedFilePath.EndsWith(".txt"))
            {
                await Task.Run(() => File.WriteAllText(selectedFilePath, textEditorText));
            }

            Write.ToEventLog("", "done");
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            FindAndReplaceWindow.ShowForReplace(textEditor);
        }

        private void BtnRedo_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Redo();
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Undo();
        }

        private async Task PlayVideo(string path)
        {
            try
            {
                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = new Uri(path);
                mediaElement.Play();
            }
            catch
            {
                await ChangeInterface("");
                Write.ToEventLog("Unable to load video preview! Exporting and importing should still work fine.", "error");
            }
        }

        private async Task RenameToBik()
        {
            string selectedFilePath = "",
                   selectedFileName = "";

            mediaElement.Stop();
            mediaElement.Close();
            mediaElement.Source = null;

            if (Vars.isDataTreeView == true)
            {
                selectedFilePath = Dirs.selectedFilePathData;
                selectedFileName = Dirs.selectedFileNameData;
            }
            else
            {
                selectedFilePath = Dirs.selectedFilePathMod;
                selectedFileName = Dirs.selectedFileNameMod;
            }

            await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));

            await Task.Delay(500);

            if (File.Exists(selectedFilePath.Replace(".binkmemory", ".mp4")))
                await Task.Run(() => FileSystem.RenameFile(selectedFilePath.Replace(".binkmemory", ".mp4"), selectedFileName.Replace(".binkmemory", ".bik")));
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            mediaElement.Close();
        }

        private void BtnPlayMedia_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
        }

        private void BtnPauseMedia_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
        }

        private void BtnStopMedia_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            mediaElement.Close();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = slider.Value;
        }

        Stream mediaStream;

        private async Task DisposeMediaStream()
        {
            if (mediaStream != null)
            {
                mediaStream.Close();
                mediaStream.Dispose();
                mediaStream = null;
                await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));
            }
        }

        private void XMLEditor_TextChanged(object sender, EventArgs e)
        {
            /*if (Vars.isDataTreeView == false || Vars.isGame == false)
                btnSave.IsEnabled = true;
            else
                btnSave.IsEnabled = false;*/
        }

        private async Task ChangeInterface (string format)
        {
            if (format == "dbx" || format == "ini" || format == "txt")
            {
                if (mediaStream != null)
                {
                    mediaStream.Close();
                    mediaStream.Dispose();
                    mediaStream = null;
                }

                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = null;
                mediaElement.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
                textEditor.Visibility = Visibility.Visible;
                txtPreview.Text = "Text Editor";
                slider.Visibility = Visibility.Hidden;
                btnPlayMedia.Visibility = Visibility.Hidden;
                btnPauseMedia.Visibility = Visibility.Hidden;
                btnStopMedia.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
                btnUndo.Visibility = Visibility.Visible;
                btnRedo.Visibility = Visibility.Visible;
                btnSearch.Visibility = Visibility.Visible;

                await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));
            }
            else if (format == "texture" || format == "ps3texture" || format == "xenontexture")
            {
                if (mediaStream != null)
                {
                    mediaStream.Close();
                    mediaStream.Dispose();
                    mediaStream = null;
                }

                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = null;
                mediaElement.Visibility = Visibility.Hidden;
                textEditor.Text = "";
                textEditor.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Visible;
                txtPreview.Text = "Texture Preview";
                slider.Visibility = Visibility.Hidden;
                btnPlayMedia.Visibility = Visibility.Hidden;
                btnPauseMedia.Visibility = Visibility.Hidden;
                btnStopMedia.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUndo.Visibility = Visibility.Hidden;
                btnRedo.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;

                await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));
            }
            else if (format == "video")
            {
                if (mediaStream != null)
                {
                    mediaStream.Close();
                    mediaStream.Dispose();
                    mediaStream = null;
                }

                textEditor.Text = "";
                textEditor.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
                mediaElement.Visibility = Visibility.Visible;
                txtPreview.Text = "Video Preview";
                slider.Visibility = Visibility.Visible;
                btnPlayMedia.Visibility = Visibility.Visible;
                btnPauseMedia.Visibility = Visibility.Visible;
                btnStopMedia.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
                btnUndo.Visibility = Visibility.Hidden;
                btnRedo.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;

                await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));
            }
            else
            {
                if (mediaStream != null)
                {
                    mediaStream.Close();
                    mediaStream.Dispose();
                    mediaStream = null;
                }

                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = null;
                mediaElement.Visibility = Visibility.Hidden;
                textEditor.Text = "";
                textEditor.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
                txtPreview.Text = "Preview";
                slider.Visibility = Visibility.Hidden;
                btnPlayMedia.Visibility = Visibility.Hidden;
                btnPauseMedia.Visibility = Visibility.Hidden;
                btnStopMedia.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUndo.Visibility = Visibility.Hidden;
                btnRedo.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;

                await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));
            }

            if (format == "dbx")
            {
                /*if (Vars.isDataTreeView == true && Vars.isModAvailable == true)
                    btnCopyToMod.IsEnabled = true;
                else
                    btnCopyToMod.IsEnabled = false;*/

                if (Vars.isDataTreeView == true && Vars.isGameProfile == true)
                    btnImport.IsEnabled = false;
                else
                    btnImport.IsEnabled = true;

                btnExport.IsEnabled = true;
                btnOpenFileLocation.IsEnabled = true;
            }
            else if (format == "ini" || format == "txt")
            {
                if (Vars.isDataTreeView == true && Vars.isGameProfile == true)
                    btnImport.IsEnabled = false;
                else
                    btnImport.IsEnabled = true;

                btnExport.IsEnabled = false;
                btnOpenFileLocation.IsEnabled = true;
            }
            else if (format == "texture" || format == "ps3texture" || format == "xenontexture" || format == "heightmap")
            {
                if (Vars.isDataTreeView == true && Vars.isGameProfile == true)
                    btnImport.IsEnabled = false;
                else
                    if (format == "texture")
                    btnImport.IsEnabled = true;
                    else
                    btnImport.IsEnabled = false;

                btnExport.IsEnabled = true;
                btnOpenFileLocation.IsEnabled = true;
            }
            else if (format == "video")
            {
                if (Vars.isDataTreeView == true && Vars.isGameProfile == true)
                    btnImport.IsEnabled = false;
                else
                    btnImport.IsEnabled = true;

                btnExport.IsEnabled = true;
                btnOpenFileLocation.IsEnabled = true;
            }
            else
            {
                string selectedFilePath = "";

                if (Vars.isDataTreeView == true)
                    selectedFilePath = Dirs.selectedFilePathData;
                else
                    selectedFilePath = Dirs.selectedFilePathMod;

                if (selectedFilePath.Contains("."))
                {
                    btnExport.IsEnabled = false;
                    btnImport.IsEnabled = false;
                    btnOpenFileLocation.IsEnabled = true;
                }
                else
                {
                    btnExport.IsEnabled = false;
                    btnImport.IsEnabled = false;
                    btnOpenFileLocation.IsEnabled = false;
                }
            }
        }

        private void InitializeStartup()
        {            
            Elements.SetElements(txtBoxEventLog, txtBoxInformation);
            Vars.SetFbrbFiles();

            image.Margin = new Thickness(235, 57, 187, 151);
            mediaElement.Margin = new Thickness(235, 57, 187, 151);
            textEditor.Margin = new Thickness(235, 57, 187, 151);
            slider.Margin = new Thickness(0, 30, 262, 0);
            btnPlayMedia.Margin = new Thickness(0, 30, 187, 0);
            btnPauseMedia.Margin = new Thickness(0, 30, 211, 0);
            btnStopMedia.Margin = new Thickness(0, 30, 235, 0);
            btnSearch.Margin = new Thickness(0, 30, 187, 0);
            btnRedo.Margin = new Thickness(0, 30, 211, 0);
            btnUndo.Margin = new Thickness(0, 30, 235, 0);
            btnSave.Margin = new Thickness(0, 30, 259, 0);

            btnArchiveMod.IsEnabled = false;
            btnArchiveFbrb.IsEnabled = false;
            btnExport.IsEnabled = false;
            btnImport.IsEnabled = false;
            btnOpenFileLocation.IsEnabled = false;

            slider.Visibility = Visibility.Hidden;
            btnPlayMedia.Visibility = Visibility.Hidden;
            btnPauseMedia.Visibility = Visibility.Hidden;
            btnStopMedia.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Hidden;
            btnUndo.Visibility = Visibility.Hidden;
            btnRedo.Visibility = Visibility.Hidden;
            btnSearch.Visibility = Visibility.Hidden;

            txtBoxEventLog.Document.Blocks.Clear();
            txtBoxInformation.Document.Blocks.Clear();

            mediaElement.Visibility = Visibility.Hidden;
            image.Visibility = Visibility.Hidden;
            textEditor.Visibility = Visibility.Hidden;
            //textEditor.TextArea.TextView.LinkTextForegroundBrush = Brushes.DodgerBlue;
            textEditor.Options.EnableEmailHyperlinks = false;
            textEditor.Options.EnableHyperlinks = false;
            //textEditor.Options.HighlightCurrentLine = true;
        }

        private void BtnDataDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.isGameProfile == false)
            {
                if (treeViewDataExplorer.SelectedItem != null)
                {
                    File.Delete(Dirs.selectedFilePathData);

                    var item = treeViewDataExplorer.SelectedItem as TreeViewItem;
                    var parent = (treeViewDataExplorer.SelectedItem as TreeViewItem).Parent as TreeViewItem;

                    parent.Items.Remove(item);
                }
            }
            else
            {
                Write.ToEventLog("You can't delete a file from a game profile!", "warning");
            }
        }

        private void BtnDataRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewDataExplorer.HasItems == true)
                Tree.Populate(treeViewDataExplorer, Dirs.filesPathData);
        }

        private void BtnModDelete_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewModExplorer.SelectedItem != null)
            {
                File.Delete(Dirs.selectedFilePathMod);

                var item = treeViewModExplorer.SelectedItem as TreeViewItem;
                var parent = (treeViewModExplorer.SelectedItem as TreeViewItem).Parent as TreeViewItem;

                parent.Items.Remove(item);
            }
        }

        private async void BtnModRestore_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewModExplorer.SelectedItem != null)
            {
                Write.ToEventLog("Restoring original file...", "");

                await ChangeInterface("");

                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.filesPathMod));

                string filePathData = "";

                if (Vars.isGameProfile == false)
                {
                    foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                    {
                        if (Dirs.filePath.Contains(kvp.Value))
                        {
                            filePathData = Dirs.filePath.Replace(kvp.Value + @"\", "");
                            break;
                        }
                    }
                }
                else
                {
                    filePathData = Dirs.filePath;
                }

                filePathData = Dirs.filesPathData + filePathData;

                if (File.Exists(filePathData))
                {
                    if (File.Exists(Dirs.selectedFilePathMod))
                        await Task.Run(() => File.Delete(Dirs.selectedFilePathMod));

                    await Task.Run(() => File.Copy(filePathData, Dirs.selectedFilePathMod));
                }
                else
                {
                    MessageBox.Show("Unable to locate the original file!\nOpen the correct game profile or fbrb archive.", "Error!");
                }

                var item = treeViewModExplorer.SelectedItem as TreeViewItem;

                item.IsSelected = false;
                item.IsSelected = true;

                Write.ToEventLog("", "done");
            }
        }

        private void BtnModRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewModExplorer.HasItems == true)
                Tree.Populate(treeViewModExplorer, Dirs.filesPathMod);
        }

        private void BtnVisitHeico_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.nexusmods.com/battlefieldbadcompany2/users/45260312");
        }

        private void BtnJoinDiscord_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.me/battlefieldmodding");
        }

        private void BtnReportBug_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("https://www.nexusmods.com/battlefieldbadcompany2/mods/4?tab=bugs");
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.Owner = this;
            infoWindow.ShowDialog();
        }
    }
}

