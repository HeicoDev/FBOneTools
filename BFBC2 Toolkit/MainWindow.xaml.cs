using System;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO;
using MahApps.Metro.Controls;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using BFBC2_Toolkit.Windows;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Functions;
using BFBC2_Toolkit.Tools;
using BFBC2_Toolkit.Helpers;
using System.Collections.Generic;

/// <summary>
/// BFBC2 Toolkit 
/// Licensed under GNU GPLv3
/// By Nico Hellmund 
/// Aka Heico
/// </summary>

namespace BFBC2_Toolkit
{
    public partial class MainWindow : MetroWindow
    {
        private FoldingManager foldingManager;
        private XmlFoldingStrategy foldingStrategy;

        public MainWindow()
        {
            InitializeComponent();

            textEditor.TextArea.TextEntering += TextEditor_TextEntering;
            textEditor.TextArea.TextEntered += TextEditor_TextEntered;

            #if DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, arguments) =>
            {
                MessageBox.Show("Unhandled exception: " + arguments.ExceptionObject);
            };
            #endif
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeStartup();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
                ResetGridSizes();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height > e.NewSize.Height || e.PreviousSize.Width > e.NewSize.Width)
                ResetGridSizes();
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
            MessageBox.Show("Note: Game profiles are still WIP!\n\nKeep in mind that adding a game can take up to an hour.\nLoading a game profile afterwards takes only a few minutes.\n\nAlso make sure that enough disk space is available.\nA game profile will take up several GBs of space.", "Warning");

            var ofd = new OpenFileDialog();
            ofd.Filter = "exe file (.exe)|*.exe";
            ofd.Title = "Select game or server executable...";

            if (ofd.ShowDialog() == true)
            {
                EnableInterface(false);

                progressRing.IsActive = true;

                await Profile.Add(ofd);

                progressRing.IsActive = false;

                EnableInterface(true);

                Write.ToEventLog("You can select your game profile now.", "done");
            }           

            //Maybe load game profile afterwards?
            //Vars.isGameProfile = true;
        }

        private void BtnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            var selectGameWindow = new SelectGameWindow();
            selectGameWindow.Owner = this;
            selectGameWindow.ShowDialog();

            if (Vars.IsGameProfile)
                btnArchiveFbrb.IsEnabled = false;
        }

        private async void BtnCreateMod_Click(object sender, RoutedEventArgs e)
        {
            await MediaStream.Dispose();

            var createModWindow = new CreateModWindow();
            createModWindow.Owner = this;
            createModWindow.ShowDialog();

            if (treeViewModExplorer.HasItems)
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

                    EnableInterface(false);

                    progressRing.IsActive = true;

                    await Mod.OpenProject(ofd);

                    btnArchiveMod.IsEnabled = true;

                    progressRing.IsActive = false;

                    EnableInterface(true);

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

                EnableInterface(false);

                progressRing.IsActive = true;

                await Mod.Extract(ofd);

                btnArchiveMod.IsEnabled = true;

                progressRing.IsActive = false;

                EnableInterface(true);

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnArchiveMod_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Archiving mod...", "");

            EnableInterface(false);

            progressRing.IsActive = true;

            await Mod.Archive();

            progressRing.IsActive = false;

            EnableInterface(true);

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

                EnableInterface(false);

                progressRing.IsActive = true;

                await Fbrb.Extract(ofd);

                btnArchiveFbrb.IsEnabled = true;

                progressRing.IsActive = false;

                EnableInterface(true);

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnArchiveFbrb_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("This may take a while. Archiving fbrb archive, please wait...", "");

            EnableInterface(false);

            progressRing.IsActive = true;

            await Fbrb.Archive();

            progressRing.IsActive = false;

            EnableInterface(true);

            Write.ToEventLog("", "done");
        }

        private async void BtnCopyToMod_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.IsDataTreeView && Vars.IsModAvailable && treeViewDataExplorer.SelectedItem != null)
            {
                Write.ToEventLog("Copying file...", "");

                EnableInterface(false);

                progressRing.IsActive = true;

                await SelectedFile.CopyToMod();

                progressRing.IsActive = false;

                EnableInterface(true);

                Write.ToEventLog("", "done");
            }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Exporting file...", "");

            EnableInterface(false);

            progressRing.IsActive = true;

            await SelectedFile.Export();

            progressRing.IsActive = false;

            EnableInterface(true);

            Write.ToEventLog("Exported file to output folder.", "done");
        }

        private async void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            Write.ToEventLog("Importing file...", "");

            EnableInterface(false);

            progressRing.IsActive = true;

            await SelectedFile.Import();

            progressRing.IsActive = false;

            EnableInterface(true);

            Write.ToEventLog("Imported file successfully.", "done");
        }

        private void BtnOpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile.OpenFileLocation();
        }

        private async void DataExplorer_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vars.IsDataTreeView = true;

            if (Dirs.SelectedFilePathData != null && Dirs.SelectedFilePathData.EndsWith(".binkmemory"))
                await SelectedFile.RenameToBik();

            if (treeViewModExplorer.SelectedItem != null)
                (treeViewModExplorer.SelectedItem as CustomTreeViewItem).IsSelected = false;

            await HandleSelectedItem(treeViewDataExplorer);
        }

        private async void ModExplorer_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vars.IsDataTreeView = false;

            if (Dirs.SelectedFilePathMod != null && Dirs.SelectedFilePathMod.EndsWith(".binkmemory"))
                await SelectedFile.RenameToBik();

            if (treeViewDataExplorer.SelectedItem != null)
                (treeViewDataExplorer.SelectedItem as CustomTreeViewItem).IsSelected = false;

            await HandleSelectedItem(treeViewModExplorer);
        }

        private async Task HandleSelectedItem(TreeView treeView)
        {
            try
            {
                treeView.Focus();

                var tvi = treeView.SelectedItem as CustomTreeViewItem;

                if (tvi != null)
                {
                    progressRing.IsActive = true;                                        

                    string selectedFileName = tvi.Name,
                           selectedFilePath = tvi.Path,
                           filesPath = String.Empty;

                    if (Vars.IsDataTreeView)
                    {
                        Dirs.SelectedFileNameData = selectedFileName;
                        Dirs.SelectedFilePathData = selectedFilePath;
                        filesPath = Dirs.FilesPathData;
                    }
                    else
                    {
                        Dirs.SelectedFileNameMod = selectedFileName;
                        Dirs.SelectedFilePathMod = selectedFilePath;
                        filesPath = Dirs.FilesPathMod;
                    }

                    if (selectedFilePath.Contains(filesPath))
                        Dirs.FilePath = selectedFilePath.Replace(filesPath, "");

                    if (selectedFileName.EndsWith(".dbx"))
                    {
                        await ChangeInterface(".dbx");
                        Write.ToInfoBox(tvi);

                        if (!File.Exists(selectedFilePath.Replace(".dbx", ".xml")))
                        {
                            var process = Process.Start(Settings.PathToPython, "\"" + Dirs.scriptDBX + "\" \"" + selectedFilePath + "\"");
                            await Task.Run(() => process.WaitForExit());
                        }

                        if (Settings.TxtEdHighlightSyntax)
                            using (var reader = new XmlTextReader(Dirs.syntaxXML))
                                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath.Replace(".dbx", ".xml")));
                        textEditor.ScrollToHome();

                        ApplyCodeFolding();
                    }
                    else if (selectedFileName.EndsWith(".dbmanifest"))
                    {
                        await ChangeInterface(".dbmanifest");
                        Write.ToInfoBox(tvi);

                        if (Settings.TxtEdHighlightSyntax)
                            using (var reader = new XmlTextReader(Dirs.syntaxXML))
                                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath));
                        textEditor.ScrollToHome();

                        ApplyCodeFolding();
                    }
                    else if (selectedFileName.EndsWith(".ini"))
                    {
                        await ChangeInterface(".ini");
                        Write.ToInfoBox(tvi);

                        if (Settings.TxtEdHighlightSyntax)
                            using (var reader = new XmlTextReader(Dirs.syntaxINI))
                                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath));
                        textEditor.ScrollToHome();
                    }
                    else if (selectedFileName.EndsWith(".txt"))
                    {
                        await ChangeInterface(".txt");
                        Write.ToInfoBox(tvi);

                        textEditor.SyntaxHighlighting = null;

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath));
                        textEditor.ScrollToHome();
                    }                    
                    else if (selectedFileName.EndsWith(".itexture"))
                    {
                        await ChangeInterface(".itexture");

                        string[] file = { selectedFilePath };

                        await Task.Run(() => TextureConverter.ConvertFile(file, false, false));

                        try
                        {
                            image.Source = BitmapHelper.LoadImage(selectedFilePath.Replace(".itexture", ".dds"));
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (selectedFileName.EndsWith(".ps3texture"))
                    {
                        await ChangeInterface(".ps3texture");

                        string[] file = { selectedFilePath };

                        await Task.Run(() => TextureConverter.ConvertFile(file, false, false));

                        try
                        {
                            image.Source = BitmapHelper.LoadImage(selectedFilePath.Replace(".ps3texture", ".dds"));
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (selectedFileName.EndsWith(".xenontexture"))
                    {
                        await ChangeInterface(".xenontexture");

                        string[] file = { selectedFilePath };

                        await Task.Run(() => TextureConverter.ConvertFile(file, false, false));

                        try
                        {
                            image.Source = BitmapHelper.LoadImage(selectedFilePath.Replace(".xenontexture", ".dds"));
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }

                        Write.ToInfoBox(tvi);
                    }
                    else if (selectedFileName.EndsWith(".terrainheightfield"))
                    {
                        await ChangeInterface(".terrainheightfield");
                        Write.ToInfoBox(tvi);

                        string[] file = { selectedFilePath };

                        if (!File.Exists(selectedFilePath.Replace(".terrainheightfield", ".raw")))
                            await Task.Run(() => TextureConverter.ConvertFile(file, false, false));
                    }
                    else if (selectedFileName.EndsWith(".binkmemory"))
                    {
                        await ChangeInterface(".binkmemory");
                        Write.ToInfoBox(tvi);

                        string mp4 = selectedFilePath.Replace(".binkmemory", ".mp4"),
                               bik = selectedFilePath.Replace(".binkmemory", ".bik");

                        if (!File.Exists(bik))
                            await Task.Run(() => File.Copy(selectedFilePath, bik));

                        if (File.Exists(mp4))
                            await Task.Run(() => File.Delete(mp4));

                        await Task.Run(() => FileSystem.RenameFile(bik, selectedFileName.Replace(".binkmemory", ".mp4")));

                        Play.Video(mp4);
                    }
                    else if (selectedFileName.EndsWith(".swfmovie"))
                    {
                        await ChangeInterface(".swfmovie");
                        Write.ToInfoBox(tvi);
                    }
                    else
                    {
                        await ChangeInterface("");
                        Write.ToInfoBox(tvi);
                    }

                    progressRing.IsActive = false;
                }
            }
            catch (Exception ex)
            {                
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to handle selected file! See error.log", "error");

                progressRing.IsActive = false;
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.IsGameProfile)
            {
                Write.ToEventLog("You can't edit a file from a game profile!", "warning");
                return;
            }

            Write.ToEventLog("Saving file...", "");

            EnableInterface(false);

            progressRing.IsActive = true;

            await Save.TextEditorChanges();

            progressRing.IsActive = false;

            EnableInterface(true);

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
            if (mediaElement != null)
                mediaElement.Volume = slider.Value;
        }                

        private void BtnDataRefresh_Click(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);

            progressRing.IsActive = true;

            if (treeViewDataExplorer.HasItems)
                Tree.Populate(treeViewDataExplorer, Dirs.FilesPathData);

            progressRing.IsActive = false;

            EnableInterface(true);
        }

        private void BtnModRefresh_Click(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);

            progressRing.IsActive = true;

            if (treeViewModExplorer.HasItems)
                Tree.Populate(treeViewModExplorer, Dirs.FilesPathMod);

            progressRing.IsActive = false;

            EnableInterface(true);
        }

        private async void BtnDataDelete_Click(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);

            progressRing.IsActive = true;

            await SelectedFile.DeleteFile("data");

            progressRing.IsActive = false;

            EnableInterface(true);
        }

        private async void BtnModDelete_Click(object sender, RoutedEventArgs e)
        {
            EnableInterface(false);

            progressRing.IsActive = true;

            await SelectedFile.DeleteFile("mod");

            progressRing.IsActive = false;

            EnableInterface(true);
        }

        private async void BtnModRestore_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewModExplorer.SelectedItem != null)
            {
                Write.ToEventLog("Restoring original file...", "");

                EnableInterface(false);

                progressRing.IsActive = true;

                await ChangeInterface("");

                await SelectedFile.RestoreFile();

                progressRing.IsActive = false;

                EnableInterface(true);

                Write.ToEventLog("", "done");
            }
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
            Process.Start("https://github.com/HeicoDev/BFBC2Toolkit/issues");
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            var infoWindow = new InfoWindow();
            infoWindow.Owner = this;
            infoWindow.ShowDialog();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
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

        private void TextEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && UIElements.CodeComWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    //UIElements.CodeComWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private void TextEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {            
            CompletionHandler.HandleInput(e.Text);
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            /*if (Vars.isDataTreeView == false || Vars.isGame == false)
                btnSave.IsEnabled = true;
            else
                btnSave.IsEnabled = false;*/

            if (foldingStrategy != null && Settings.TxtEdCodeFolding)
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
        }

        private void ApplyCodeFolding()
        {
            if (foldingManager != null)
                FoldingManager.Uninstall(foldingManager);

            if (Settings.TxtEdCodeFolding)
            {
                foldingManager = FoldingManager.Install(textEditor.TextArea);
                foldingStrategy = new XmlFoldingStrategy();
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
        }

        private async Task ChangeInterface(string format)
        {
            if (format == ".dbx" || format == ".dbmanifest" || format == ".ini" || format == ".txt")
            {
                txtPreview.Text = "Text Editor";
                mediaElement.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;                               
                slider.Visibility = Visibility.Hidden;
                btnPlayMedia.Visibility = Visibility.Hidden;
                btnPauseMedia.Visibility = Visibility.Hidden;
                btnStopMedia.Visibility = Visibility.Hidden;
                textEditor.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                btnUndo.Visibility = Visibility.Visible;
                btnRedo.Visibility = Visibility.Visible;
                btnSearch.Visibility = Visibility.Visible;

                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = null;

                await MediaStream.Dispose();
            }
            else if (format == ".itexture" || format == ".ps3texture" || format == ".xenontexture")
            {
                txtPreview.Text = "Texture Preview";
                textEditor.Text = "";
                textEditor.Visibility = Visibility.Hidden;
                mediaElement.Visibility = Visibility.Hidden;
                slider.Visibility = Visibility.Hidden;
                btnPlayMedia.Visibility = Visibility.Hidden;
                btnPauseMedia.Visibility = Visibility.Hidden;
                btnStopMedia.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUndo.Visibility = Visibility.Hidden;
                btnRedo.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Visible;               

                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = null;

                await MediaStream.Dispose();
            }
            else if (format == ".binkmemory")
            {
                txtPreview.Text = "Video Preview";
                textEditor.Text = "";
                textEditor.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;               
                btnSave.Visibility = Visibility.Hidden;
                btnUndo.Visibility = Visibility.Hidden;
                btnRedo.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;
                mediaElement.Visibility = Visibility.Visible;
                slider.Visibility = Visibility.Visible;
                btnPlayMedia.Visibility = Visibility.Visible;
                btnPauseMedia.Visibility = Visibility.Visible;
                btnStopMedia.Visibility = Visibility.Visible;

                await MediaStream.Dispose();
            }
            else
            {
                txtPreview.Text = "Preview";
                textEditor.Text = "";
                textEditor.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;                
                slider.Visibility = Visibility.Hidden;
                btnPlayMedia.Visibility = Visibility.Hidden;
                btnPauseMedia.Visibility = Visibility.Hidden;
                btnStopMedia.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUndo.Visibility = Visibility.Hidden;
                btnRedo.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;
                mediaElement.Visibility = Visibility.Hidden;
                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = null;

                await MediaStream.Dispose();
            }

            ChangeBottomButtons(format);
        }

        private void EnableInterface(bool enableButtons)
        {
            if (enableButtons)
            {
                treeViewDataExplorer.IsEnabled = true;
                treeViewModExplorer.IsEnabled = true;

                btnAddGame.IsEnabled = true;
                btnSelectGame.IsEnabled = true;
                btnCreateMod.IsEnabled = true;
                btnOpenMod.IsEnabled = true;
                btnExtractMod.IsEnabled = true;
                btnExtractFbrb.IsEnabled = true;
                btnSettings.IsEnabled = true;
                btnDataDelete.IsEnabled = true;
                btnModDelete.IsEnabled = true;
                btnCopyToMod.IsEnabled = true;
                btnModRestore.IsEnabled = true;
                btnDataRefresh.IsEnabled = true;
                btnModRefresh.IsEnabled = true;
                btnSave.IsEnabled = true;

                if (Vars.IsModAvailable)
                    btnArchiveMod.IsEnabled = true;

                if (Vars.IsDataAvailable && !Vars.IsGameProfile)
                    btnArchiveFbrb.IsEnabled = true;

                string selectedFileName = "";

                if (Vars.IsDataTreeView)
                    selectedFileName = Dirs.SelectedFilePathData;
                else
                    selectedFileName = Dirs.SelectedFilePathMod;

                string selectedFileExt = Path.GetExtension(selectedFileName);

                ChangeBottomButtons(selectedFileExt);
            }
            else
            {
                treeViewDataExplorer.IsEnabled = false;
                treeViewModExplorer.IsEnabled = false;

                btnAddGame.IsEnabled = false;
                btnSelectGame.IsEnabled = false;
                btnCreateMod.IsEnabled = false;
                btnOpenMod.IsEnabled = false;
                btnExtractMod.IsEnabled = false;
                btnExtractFbrb.IsEnabled = false;
                btnSettings.IsEnabled = false;
                btnArchiveMod.IsEnabled = false;
                btnArchiveFbrb.IsEnabled = false;
                btnImport.IsEnabled = false;
                btnExport.IsEnabled = false;
                btnOpenFileLocation.IsEnabled = false;
                btnDataDelete.IsEnabled = false;
                btnModDelete.IsEnabled = false;
                btnCopyToMod.IsEnabled = false;
                btnModRestore.IsEnabled = false;
                btnDataRefresh.IsEnabled = false;
                btnModRefresh.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
        }

        private void ChangeBottomButtons(string format)
        {
            if (format == ".dbx")
            {
                if (Vars.IsDataTreeView && Vars.IsGameProfile)
                    btnImport.IsEnabled = false;
                else
                    btnImport.IsEnabled = true;

                btnExport.IsEnabled = true;
                btnOpenFileLocation.IsEnabled = true;
            }
            else if (format == ".dbmanifest" || format == ".ini" || format == ".txt")
            {
                btnImport.IsEnabled = false;
                btnExport.IsEnabled = false;
                btnOpenFileLocation.IsEnabled = true;
            }
            else if (format == ".itexture" || format == ".ps3texture" || format == ".xenontexture" || format == ".terrainheightfield")
            {
                if (Vars.IsDataTreeView && Vars.IsGameProfile)
                    btnImport.IsEnabled = false;
                else if (format == ".itexture")
                    btnImport.IsEnabled = true;
                else
                    btnImport.IsEnabled = false;

                btnExport.IsEnabled = true;
                btnOpenFileLocation.IsEnabled = true;
            }
            else if (format == ".binkmemory")
            {
                if (Vars.IsDataTreeView && Vars.IsGameProfile)
                    btnImport.IsEnabled = false;
                else
                    btnImport.IsEnabled = true;

                btnExport.IsEnabled = true;
                btnOpenFileLocation.IsEnabled = true;
            }
            else if (format == ".swfmovie")
            {
                btnImport.IsEnabled = false;
                btnExport.IsEnabled = true;
                btnOpenFileLocation.IsEnabled = true;
            }
            else
            {
                string selectedFileName = "";

                if (Vars.IsDataTreeView)
                    selectedFileName = Dirs.SelectedFilePathData;
                else
                    selectedFileName = Dirs.SelectedFilePathMod;

                if (selectedFileName != null && selectedFileName.Contains("."))
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

        private async Task InitializeStartup()
        {
            if (!File.Exists(Settings.PathToPython))
            {
                MessageBox.Show("Unable to locate Python 2.7 installation!\nPlease select pythonw.exe...", "Error");

                string path = SettingsHandler.ChangePythonPath();

                if (path == String.Empty)
                {
                    MessageBox.Show("Unable to locate pythonw.exe!\nPress 'OK' to close the app.", "Error");

                    Environment.Exit(0);
                }
                else
                {
                    SettingsHandler.Save();
                }
            }

            try
            {
                await Task.Run(() => Create.PrecreateDirs());
                await Task.Run(() => Create.ConfigFiles());
                await Task.Run(() => CleanUp.StartUp());
                SettingsHandler.Load();
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                MessageBox.Show("Unable to initialize startup! See error.log\nPress 'OK' to close the app.", "Error");
                Environment.Exit(0);
            }

            UIElements.TxtBoxEventLog = txtBoxEventLog;
            UIElements.TxtBoxInformation = txtBoxInformation;
            UIElements.TextEditor = textEditor;
            UIElements.TreeViewDataExplorer = treeViewDataExplorer;
            UIElements.TreeViewModExplorer = treeViewModExplorer;
            UIElements.MediaElement = mediaElement;
            UIElements.ImageElement = image;

            Vars.SetFbrbFiles();

            image.Margin = new Thickness(0, 31, 0, 27);
            mediaElement.Margin = new Thickness(0, 31, 0, 27);
            textEditor.Margin = new Thickness(0, 31, 0, 27);
            slider.Margin = new Thickness(0, 4, 77, 0);
            btnPlayMedia.Margin = new Thickness(0, 4, 1, 0);
            btnPauseMedia.Margin = new Thickness(0, 4, 25, 0);
            btnStopMedia.Margin = new Thickness(0, 4, 49, 0);
            btnSearch.Margin = new Thickness(0, 4, 0, 0);
            btnRedo.Margin = new Thickness(0, 4, 24, 0);
            btnUndo.Margin = new Thickness(0, 4, 48, 0);
            btnSave.Margin = new Thickness(0, 4, 72, 0);

            btnArchiveMod.IsEnabled = false;
            btnArchiveFbrb.IsEnabled = false;
            btnExport.IsEnabled = false;
            btnImport.IsEnabled = false;
            btnOpenFileLocation.IsEnabled = false;

            textEditor.Visibility = Visibility.Hidden;
            mediaElement.Visibility = Visibility.Hidden;
            image.Visibility = Visibility.Hidden;            
            slider.Visibility = Visibility.Hidden;
            btnPlayMedia.Visibility = Visibility.Hidden;
            btnPauseMedia.Visibility = Visibility.Hidden;
            btnStopMedia.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Hidden;
            btnUndo.Visibility = Visibility.Hidden;
            btnRedo.Visibility = Visibility.Hidden;
            btnSearch.Visibility = Visibility.Hidden;

            mediaElement.Volume = slider.Value;
           
            //I should move this text editor stuff to XAML
            textEditor.TextArea.TextView.LinkTextForegroundBrush = Brushes.DodgerBlue;
            textEditor.TextArea.SelectionBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF686868"));
            textEditor.TextArea.SelectionBorder.Brush = Brushes.Gray;
            textEditor.TextArea.SelectionBorder.LineJoin = PenLineJoin.Bevel;
            textEditor.Options.EnableEmailHyperlinks = false;
            textEditor.Options.EnableHyperlinks = true;
            textEditor.Options.RequireControlModifierForHyperlinkClick = false;

            txtBoxEventLog.Document.Blocks.Clear();
            txtBoxInformation.Document.Blocks.Clear();
        }

        private void ResetGridSizes()
        {
            //Workaround to fix an issue with some Grid/UI sizes when decreasing the size of the main window
            gridMainElements.ColumnDefinitions[0].Width = new GridLength(229, GridUnitType.Star);
            gridMainElements.ColumnDefinitions[2].Width = new GridLength(720, GridUnitType.Star);

            gridData.RowDefinitions[0].Height = new GridLength(245, GridUnitType.Star);
            gridData.RowDefinitions[2].Height = new GridLength(147, GridUnitType.Star);

            gridPreviewLogProp.RowDefinitions[0].Height = new GridLength(274, GridUnitType.Star);
            gridPreviewLogProp.RowDefinitions[2].Height = new GridLength(121, GridUnitType.Star);

            gridPreviewProp.ColumnDefinitions[0].Width = new GridLength(500, GridUnitType.Star);
            gridPreviewProp.ColumnDefinitions[2].Width = new GridLength(182, GridUnitType.Star);
        }       
    }
}

