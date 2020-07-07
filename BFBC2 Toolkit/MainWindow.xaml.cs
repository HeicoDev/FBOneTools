using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Diagnostics;
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
                    await Save.TextEditorChanges();
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

            Write.ToEventLog("You can select your game profile now.", "done");

            //Vars.isGameProfile = true;
        }

        private void BtnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            var selectGameWindow = new SelectGameWindow();
            selectGameWindow.Owner = this;
            selectGameWindow.ShowDialog();

            if (Vars.isGameProfile)
                btnArchiveFbrb.IsEnabled = false;
        }

        private async void BtnCreateMod_Click(object sender, RoutedEventArgs e)
        {
            await MediaStream.Dispose();

            var createModWindow = new CreateModWindow();
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

                    await Mod.OpenProject(ofd);

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

                await Mod.Extract(ofd);

                btnArchiveMod.IsEnabled = true;

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnArchiveMod_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Archiving mod...", "");

            await Mod.Archive();

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

                await Fbrb.Extract(ofd);

                btnArchiveFbrb.IsEnabled = true;

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnArchiveFbrb_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("This may take a while. Archiving fbrb archive, please wait...", "");

            await Fbrb.Archive();

            Write.ToEventLog("", "done");
        }

        private async void BtnCopyToMod_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.isDataTreeView == true && Vars.isModAvailable == true && treeViewDataExplorer.SelectedItem != null)
            {
                Write.ToEventLog("Copying file...", "");

                await SelectedFile.CopyToMod();

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Exporting file...", "");

            await SelectedFile.Export();

            Write.ToEventLog("Exported file to output folder.", "done");
        }

        private async void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            await SelectedFile.Import();
        }

        private void BtnOpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile.OpenFileLocation();
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
                    await SelectedFile.RenameToBik();

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

                        using (var reader = new XmlTextReader(Dirs.syntaxXML))
                            textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathData.Replace(".dbx", ".xml")));
                        textEditor.ScrollToHome();
                    }
                    else if (Dirs.selectedFileNameData.EndsWith(".ini"))
                    {
                        await ChangeInterface("ini");
                        Write.ToInfoBox(tvi);

                        using (var reader = new XmlTextReader(Dirs.syntaxINI))
                            textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

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
                            MediaStream.stream = new FileStream(Dirs.selectedFilePathData.Replace(".itexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = MediaStream.stream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
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
                            MediaStream.stream = new FileStream(Dirs.selectedFilePathData.Replace(".ps3texture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = MediaStream.stream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
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
                            MediaStream.stream = new FileStream(Dirs.selectedFilePathData.Replace(".xenontexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = MediaStream.stream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
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

                        Play.Video(mp4);                   
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
                Vars.isDataTreeView = false;

                if (treeViewDataExplorer.SelectedItem != null)
                    (treeViewDataExplorer.SelectedItem as CustomTreeViewItem).IsSelected = false;

                treeViewModExplorer.Focus();

                if (Dirs.selectedFilePathMod.EndsWith(".binkmemory"))
                    await SelectedFile.RenameToBik();

                var tvi = treeViewModExplorer.SelectedItem as CustomTreeViewItem;

                if (tvi != null)
                {
                    Dirs.selectedFileNameMod = tvi.Name;
                    Dirs.selectedFilePathMod = tvi.Path;

                    if (Dirs.selectedFilePathMod.Contains(Dirs.filesPathMod))
                        Dirs.filePath = Dirs.selectedFilePathMod.Replace(Dirs.filesPathMod, "");

                    if (Dirs.selectedFileNameMod.EndsWith(".dbx"))
                    {
                        await ChangeInterface("dbx");
                        Write.ToInfoBox(tvi);

                        if (!File.Exists(Dirs.selectedFilePathMod.Replace(".dbx", ".xml")))
                        {
                            var process = Process.Start(Dirs.scriptDBX, "\"" + Dirs.selectedFilePathMod);
                            await Task.Run(() => process.WaitForExit());
                        }

                        using (var reader = new XmlTextReader(Dirs.syntaxXML))
                            textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathMod.Replace(".dbx", ".xml")));
                        textEditor.ScrollToHome();
                    }
                    else if (Dirs.selectedFileNameMod.EndsWith(".ini"))
                    {
                        await ChangeInterface("ini");
                        Write.ToInfoBox(tvi);

                        using (var reader = new XmlTextReader(Dirs.syntaxINI))
                            textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathMod));
                        textEditor.ScrollToHome();
                    }
                    else if (Dirs.selectedFileNameMod.EndsWith(".txt"))
                    {
                        await ChangeInterface("txt");
                        Write.ToInfoBox(tvi);

                        textEditor.SyntaxHighlighting = null;

                        textEditor.Text = await Task.Run(() => File.ReadAllText(Dirs.selectedFilePathMod));
                        textEditor.ScrollToHome();
                    }
                    else if (Dirs.selectedFileNameMod.EndsWith(".itexture"))
                    {
                        await ChangeInterface("texture");

                        string[] file = { Dirs.selectedFilePathMod };

                        //if (!File.Exists(Dirs.selectedFilePathMod.Replace(".itexture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            MediaStream.stream = new FileStream(Dirs.selectedFilePathMod.Replace(".itexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = MediaStream.stream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (Dirs.selectedFileNameMod.EndsWith(".ps3texture"))
                    {
                        await ChangeInterface("ps3texture");

                        string[] file = { Dirs.selectedFilePathMod };

                        //if (!File.Exists(Dirs.selectedFilePathMod.Replace(".ps3texture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            MediaStream.stream = new FileStream(Dirs.selectedFilePathMod.Replace(".ps3texture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = MediaStream.stream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (Dirs.selectedFileNameMod.EndsWith(".xenontexture"))
                    {
                        await ChangeInterface("xenontexture");

                        string[] file = { Dirs.selectedFilePathMod };

                        //if (!File.Exists(Dirs.selectedFilePathMod.Replace(".ps3texture", ".dds")))
                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            MediaStream.stream = new FileStream(Dirs.selectedFilePathMod.Replace(".xenontexture", ".dds"), FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = MediaStream.stream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (Dirs.selectedFileNameMod.EndsWith(".terrainheightfield"))
                    {
                        await ChangeInterface("heightmap");
                        Write.ToInfoBox(tvi);

                        string[] file = { Dirs.selectedFilePathMod };

                        if (!File.Exists(Dirs.selectedFilePathMod.Replace(".terrainheightfield", ".raw")))
                            await Task.Run(() => TextureConverter.ConvertFile(file, false));
                    }
                    else if (Dirs.selectedFileNameMod.EndsWith(".binkmemory"))
                    {
                        await ChangeInterface("video");
                        Write.ToInfoBox(tvi);

                        string mp4 = Dirs.selectedFilePathMod.Replace(".binkmemory", ".mp4"),
                               bik = Dirs.selectedFilePathMod.Replace(".binkmemory", ".bik");

                        if (!File.Exists(bik))
                            await Task.Run(() => File.Copy(Dirs.selectedFilePathMod, bik));

                        if (File.Exists(mp4))
                            await Task.Run(() => File.Delete(mp4));

                        await Task.Run(() => FileSystem.RenameFile(bik, Dirs.selectedFileNameMod.Replace(".binkmemory", ".mp4")));

                        Play.Video(mp4);
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

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Saving file...", "");

            await Save.TextEditorChanges();

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

        private void BtnDataRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewDataExplorer.HasItems == true)
                Tree.Populate(treeViewDataExplorer, Dirs.filesPathData);
        }

        private async void BtnDataDelete_Click(object sender, RoutedEventArgs e)
        {
            await SelectedFile.DeleteFile("data");
        }

        private async void BtnModDelete_Click(object sender, RoutedEventArgs e)
        {
            await SelectedFile.DeleteFile("mod");
        }

        private async void BtnModRestore_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewModExplorer.SelectedItem != null)
            {
                Write.ToEventLog("Restoring original file...", "");

                await ChangeInterface("");

                await SelectedFile.RestoreFile();

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
            var infoWindow = new InfoWindow();
            infoWindow.Owner = this;
            infoWindow.ShowDialog();
        }

        private void XMLEditor_TextChanged(object sender, EventArgs e)
        {
            /*if (Vars.isDataTreeView == false || Vars.isGame == false)
                btnSave.IsEnabled = true;
            else
                btnSave.IsEnabled = false;*/
        }

        private void DropdownButton_Checked(object sender, RoutedEventArgs e)
        {
            var menu = (sender as ToggleButton).ContextMenu;
            menu.PlacementTarget = sender as ToggleButton;
            menu.Placement = PlacementMode.Bottom;
            menu.IsOpen = true;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            ((sender as ContextMenu).PlacementTarget as ToggleButton).IsChecked = false;
        }

        private void DropdownButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private async Task ChangeInterface(string format)
        {
            if (format == "dbx" || format == "ini" || format == "txt")
            {
                if (MediaStream.stream != null)
                {
                    MediaStream.stream.Close();
                    MediaStream.stream.Dispose();
                    MediaStream.stream = null;
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
                if (MediaStream.stream != null)
                {
                    MediaStream.stream.Close();
                    MediaStream.stream.Dispose();
                    MediaStream.stream = null;
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
                if (MediaStream.stream != null)
                {
                    MediaStream.stream.Close();
                    MediaStream.stream.Dispose();
                    MediaStream.stream = null;
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
                if (MediaStream.stream != null)
                {
                    MediaStream.stream.Close();
                    MediaStream.stream.Dispose();
                    MediaStream.stream = null;
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
            if (!File.Exists(@"C:\Python27\python.exe"))
            {
                MessageBox.Show("Unable to locate Python 2.7 (32 bit) installation!\nPlease make sure that Python is installed to C:\\Python27", "Error");
                Environment.Exit(0);
            }

            if (!Directory.Exists(Dirs.games))
                Directory.CreateDirectory(Dirs.games);
            if (!Directory.Exists(Dirs.logs))
                Directory.CreateDirectory(Dirs.logs);
            if (!Directory.Exists(Dirs.projects))
                Directory.CreateDirectory(Dirs.projects);
            if (!Directory.Exists(Dirs.output))
            {
                Directory.CreateDirectory(Dirs.output);
                Directory.CreateDirectory(Dirs.outputDDS);
                Directory.CreateDirectory(Dirs.outputHeightmap);
                Directory.CreateDirectory(Dirs.outputiTexture);
                Directory.CreateDirectory(Dirs.outputMods);
                Directory.CreateDirectory(Dirs.outputVideo);
                Directory.CreateDirectory(Dirs.outputXML);
            }

            if (File.Exists(Dirs.errorLog))
                File.Delete(Dirs.errorLog);

            Elements.TxtBoxEventLog = txtBoxEventLog;
            Elements.TxtBoxInformation = txtBoxInformation;
            Elements.TextEditor = textEditor;
            Elements.TreeViewDataExplorer = treeViewDataExplorer;
            Elements.TreeViewModExplorer = treeViewModExplorer;
            Elements.MediaElement = mediaElement;
            Elements.ImageElement = image;

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

        private void MenuItemFileConverter_Click(object sender, RoutedEventArgs e)
        {
            var fileConverterWindow = new FileConverterWindow();
            fileConverterWindow.Owner = this;
            fileConverterWindow.Show();
        }

        private void MenuItemFilePorter_Click(object sender, RoutedEventArgs e)
        {
            var filePorterWindow = new FilePorterWindow();
            filePorterWindow.Owner = this;
            filePorterWindow.Show();
        }

        private void MenuItemCustomizer_Click(object sender, RoutedEventArgs e)
        {
            var customizerWindow = new CustomizerWindow();
            customizerWindow.Owner = this;
            customizerWindow.ShowDialog();
        }
    }
}

