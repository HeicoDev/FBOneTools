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
using ICSharpCode.AvalonEdit.Folding;
using BFBC2Toolkit.Windows;
using BFBC2Toolkit.Data;
using BFBC2Toolkit.Functions;
using BFBC2Toolkit.Tools;
using BFBC2Toolkit.Helpers;
using BFBC2Shared.Data;
using BFBC2Shared.Functions;

/// <summary>
/// BFBC2 Toolkit 
/// Licensed under GNU GPLv3
/// By Nico Hellmund 
/// Aka Heico
/// </summary>

namespace BFBC2Toolkit
{
    public partial class MainWindow : MetroWindow
    {
        private FoldingManager foldingManager;
        private XmlFoldingStrategy foldingStrategy;

        public MainWindow()
        {
            InitializeComponent();

            //textEditor.TextArea.TextEntering += TextEditor_TextEntering;
            textEditor.TextArea.TextEntered += TextEditor_TextEntered;
           
            AppDomain.CurrentDomain.UnhandledException += (sender, arguments) =>
            {
                #if DEBUG
                MessageBox.Show("Unhandled exception: " + arguments.ExceptionObject);
                #endif

                Log.Write("An unhandled exception has occurred! See error.log", "error");
                Log.Error("Unhandled exception: " + arguments.ExceptionObject);
            };            
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

                bool hasErrorOccurred = await Profile.Add(ofd);

                progressRing.IsActive = false;

                EnableInterface(true);

                if (!hasErrorOccurred)
                    Log.Write("You can select your game profile now.", "done");
            }           

            //Maybe load game profile afterwards?
            //Vars.isGameProfile = true;
        }

        private void BtnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            var selectGameWindow = new SelectGameWindow();
            selectGameWindow.Owner = this;
            selectGameWindow.ShowDialog();

            if (Globals.IsGameProfile)
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
            ofd.InitialDirectory = Dirs.Projects;

            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileName.Contains("ModInfo.ini"))
                {
                    Log.Write("Loading mod files...");

                    EnableInterface(false);

                    progressRing.IsActive = true;

                    bool hasErrorOccurred = await Mod.OpenProject(ofd);

                    btnArchiveMod.IsEnabled = true;

                    progressRing.IsActive = false;

                    EnableInterface(true);

                    if (!hasErrorOccurred)
                        Log.Write("", "done");
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
                Log.Write("Extracting mod archive...");

                EnableInterface(false);

                progressRing.IsActive = true;

                bool hasErrorOccurred = await Mod.Extract(ofd);

                btnArchiveMod.IsEnabled = true;

                progressRing.IsActive = false;

                EnableInterface(true);

                if (!hasErrorOccurred)
                    Log.Write("", "done");
            }
        }

        private async void BtnArchiveMod_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Archiving mod...");

            EnableInterface(false);

            progressRing.IsActive = true;

            bool hasErrorOccurred = await Mod.Archive();

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
        }

        private async void BtnExtractFbrb_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "fbrb file (.fbrb)|*.fbrb";
            ofd.Title = "Select fbrb archive...";

            if (ofd.ShowDialog() == true)
            {
                Log.Write("This may take a while. Extracting fbrb archive, please wait...");

                EnableInterface(false);

                progressRing.IsActive = true;

                bool hasErrorOccurred = await Fbrb.Extract(ofd);

                btnArchiveFbrb.IsEnabled = true;

                progressRing.IsActive = false;

                EnableInterface(true);

                if (!hasErrorOccurred)
                    Log.Write("", "done");
            }
        }

        private async void BtnArchiveFbrb_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("This may take a while. Archiving fbrb archive, please wait...");

            EnableInterface(false);

            progressRing.IsActive = true;

            bool hasErrorOccurred = await Fbrb.Archive();

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
        }

        private async void BtnCopyToMod_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.IsDataTreeView && Globals.IsModAvailable && treeViewDataExplorer.SelectedItem != null)
            {
                Log.Write("Copying file...");

                EnableInterface(false);

                progressRing.IsActive = true;

                bool hasErrorOccurred = await SelectedFile.CopyToMod();

                progressRing.IsActive = false;

                EnableInterface(true);

                if (!hasErrorOccurred)
                    Log.Write("", "done");
            }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Exporting file...");

            EnableInterface(false);

            progressRing.IsActive = true;

            bool hasErrorOccurred = await SelectedFile.Export();

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("Exported file to Output folder.", "done");
        }

        private async void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Importing file...");

            EnableInterface(false);

            progressRing.IsActive = true;

            bool hasErrorOccurred = await SelectedFile.Import();

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("Imported file successfully.", "done");
        }

        private void BtnOpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile.OpenFileLocation();
        }

        private async void DataExplorer_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Globals.IsDataTreeView = true;

            if (Dirs.SelectedFilePathData != null && Dirs.SelectedFilePathData.EndsWith(".binkmemory"))
                await SelectedFile.RenameToBik();

            if (treeViewModExplorer.SelectedItem != null)
                (treeViewModExplorer.SelectedItem as CustomTreeViewItem).IsSelected = false;

            await HandleSelectedItem(treeViewDataExplorer);
        }

        private async void ModExplorer_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Globals.IsDataTreeView = false;

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
                await MediaStream.Dispose();

                treeView.Focus();

                var tvi = treeView.SelectedItem as CustomTreeViewItem;

                if (tvi != null)
                {
                    progressRing.IsActive = true;                    

                    string selectedFileName = tvi.Name,
                           selectedFilePath = tvi.Path,
                           selectedFileExtension = Path.GetExtension(tvi.Path),
                           filesPath = String.Empty;

                    await ChangeInterface(selectedFileExtension);

                    if (Globals.IsDataTreeView)
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
                        if (!File.Exists(selectedFilePath.Replace(selectedFileExtension, ".xml")))
                        {
                            await Python.ExecuteScript(Dirs.ScriptDBX, selectedFilePath);
                        }

                        if (Settings.TxtEdHighlightSyntax)
                            using (var reader = new XmlTextReader(Dirs.SyntaxXML))
                                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath.Replace(selectedFileExtension, ".xml")));
                        textEditor.ScrollToHome();
                        textEditor.Visibility = Visibility.Visible;

                        ApplyCodeFolding();
                    }
                    else if (selectedFileName.EndsWith(".dbmanifest"))
                    {
                        if (Settings.TxtEdHighlightSyntax)
                            using (var reader = new XmlTextReader(Dirs.SyntaxXML))
                                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath));
                        textEditor.ScrollToHome();
                        textEditor.Visibility = Visibility.Visible;

                        ApplyCodeFolding();                       
                    }
                    else if (selectedFileName.EndsWith(".ini"))
                    {
                        if (Settings.TxtEdHighlightSyntax)
                            using (var reader = new XmlTextReader(Dirs.SyntaxINI))
                                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath));
                        textEditor.ScrollToHome();
                        textEditor.Visibility = Visibility.Visible;
                    }
                    else if (selectedFileName.EndsWith(".txt"))
                    {
                        textEditor.SyntaxHighlighting = null;

                        textEditor.Text = await Task.Run(() => File.ReadAllText(selectedFilePath));
                        textEditor.ScrollToHome();
                        textEditor.Visibility = Visibility.Visible;
                    }                    
                    else if (selectedFileName.EndsWith(".itexture") || selectedFileName.EndsWith(".ps3texture") || selectedFileName.EndsWith(".xenontexture"))
                    {                        
                        string[] file = { selectedFilePath };

                        await Task.Run(() => TextureConverter.ConvertFile(file, false, false));

                        try
                        {
                            image.Source = BitmapHelper.LoadImage(selectedFilePath.Replace(selectedFileExtension, ".dds"));
                            image.Visibility = Visibility.Visible;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Log.Write("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }
                    }
                    else if (selectedFileName.EndsWith(".terrainheightfield"))
                    {                                              
                        string[] file = { selectedFilePath };

                        await Task.Run(() => TextureConverter.ConvertFile(file, false, false));

                        try
                        {                            
                            image.Source = BitmapHelper.LoadGrayscaleImage(selectedFilePath.Replace(selectedFileExtension, ".raw"));
                            image.Visibility = Visibility.Visible;
                        }
                        catch
                        {
                            image.Visibility = Visibility.Hidden;

                            Log.Write("Unable to load heightmap preview! Exporting should still work fine.", "warning");
                        }
                    }
                    else if (selectedFileName.EndsWith(".binkmemory"))
                    {                                                
                        string mp4 = selectedFilePath.Replace(selectedFileExtension, ".mp4"),
                               bik = selectedFilePath.Replace(selectedFileExtension, ".bik");

                        try
                        {
                            if (!File.Exists(bik))
                                await Task.Run(() => File.Copy(selectedFilePath, bik));

                            if (File.Exists(mp4))
                                await Task.Run(() => File.Delete(mp4));

                            await Task.Run(() => FileSystem.RenameFile(bik, selectedFileName.Replace(selectedFileExtension, ".mp4")));

                            Play.Video(mp4);

                            mediaElement.Visibility = Visibility.Visible;
                        }
                        catch
                        {
                            mediaElement.Visibility = Visibility.Hidden;

                            Log.Write("Unable to load video preview! Exporting & importing should still work fine.", "warning");
                        }
                    }

                    Write.ToInfoBox(tvi);

                    progressRing.IsActive = false;
                }
            }
            catch (Exception ex)
            {                
                Log.Error(ex.ToString());
                Log.Write("Unable to handle selected file! See error.log", "error");

                progressRing.IsActive = false;
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.IsGameProfile && Globals.IsDataTreeView)
            {
                Log.Write("You can't edit a file from a game profile!", "warning");
                return;
            }

            Log.Write("Saving file...");

            EnableInterface(false);

            progressRing.IsActive = true;

            bool hasErrorOccurred = await Save.TextEditorChanges();

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
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

        private async void BtnDataRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!treeViewDataExplorer.HasItems)
                return;

            bool hasErrorOccurred = false;

            Log.Write("Refreshing data explorer...");

            EnableInterface(false);

            progressRing.IsActive = true;

            try
            {               
                await Tree.Populate(treeViewDataExplorer, Dirs.FilesPathData);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to refresh data explorer! See error.log", "error");

                hasErrorOccurred = true;
            }

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
        }

        private async void BtnModRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!treeViewModExplorer.HasItems)
                return;

            bool hasErrorOccurred = false;

            Log.Write("Refreshing mod explorer...");

            EnableInterface(false);

            progressRing.IsActive = true;

            try
            {
                await Tree.Populate(treeViewModExplorer, Dirs.FilesPathMod);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to refresh data explorer! See error.log", "error");

                hasErrorOccurred = true;
            }

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
        }

        private async void BtnDataDelete_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewDataExplorer.SelectedItem == null)
                return;

            Log.Write("Deleting selected file...");

            EnableInterface(false);

            progressRing.IsActive = true;

            bool hasErrorOccurred = await SelectedFile.DeleteFile("data");

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
        }

        private async void BtnModDelete_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewModExplorer.SelectedItem == null)
                return;

            Log.Write("Deleting selected file...");

            EnableInterface(false);

            progressRing.IsActive = true;

            bool hasErrorOccurred = await SelectedFile.DeleteFile("mod");

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
        }

        private async void BtnModRestore_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewModExplorer.SelectedItem == null)
                return;

            Log.Write("Restoring original file...");

            EnableInterface(false);

            progressRing.IsActive = true;

            await ChangeInterface("");

            bool hasErrorOccurred = await SelectedFile.RestoreFile();

            progressRing.IsActive = false;

            EnableInterface(true);

            if (!hasErrorOccurred)
                Log.Write("", "done");
        }        

        private void BtnVisitHeico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.nexusmods.com/battlefieldbadcompany2/users/45260312");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open link! See error.log", "error");
            }
        }

        private void BtnJoinDiscord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://discord.me/battlefieldmodding");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open link! See error.log", "error");
            }
        }

        private void BtnReportBug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.nexusmods.com/battlefieldbadcompany2/mods/15?tab=bugs");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open link! See error.log", "error");
            }
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
            if (Settings.TxtEdCodeCompletion)
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

                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Source = null;

                await MediaStream.Dispose();
            }
            else if (format == ".terrainheightfield")
            {
                txtPreview.Text = "Heightmap Preview";
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
                slider.Visibility = Visibility.Visible;
                btnPlayMedia.Visibility = Visibility.Visible;
                btnPauseMedia.Visibility = Visibility.Visible;
                btnStopMedia.Visibility = Visibility.Visible;
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

                if (Globals.IsModAvailable)
                    btnArchiveMod.IsEnabled = true;

                if (Globals.IsDataAvailable && !Globals.IsGameProfile)
                    btnArchiveFbrb.IsEnabled = true;

                string selectedFileName = "";

                if (Globals.IsDataTreeView)
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
                if (Globals.IsDataTreeView && Globals.IsGameProfile)
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
                if (Globals.IsDataTreeView && Globals.IsGameProfile)
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
                if (Globals.IsDataTreeView && Globals.IsGameProfile)
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

                if (Globals.IsDataTreeView)
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
            WindowState = WindowState.Minimized;

            var splashscreen = new SplashscreenWindow();
            splashscreen.Owner = this;
            splashscreen.Topmost = true;
            splashscreen.Show();

            SharedUIElements.TxtBoxEventLog = txtBoxEventLog;
            UIElements.TxtBoxInformation = txtBoxInformation;
            UIElements.TextEditor = textEditor;
            UIElements.TreeViewDataExplorer = treeViewDataExplorer;
            UIElements.TreeViewModExplorer = treeViewModExplorer;
            UIElements.MediaElement = mediaElement;
            UIElements.ImageElement = image;

            Dirs.SetSharedVars();
            Globals.SetSharedVars();
            Globals.SetFbrbFiles();

            try
            {
                await Task.Run(() => CleanUp.StartUp());
                await Task.Run(() => Create.PrecreateDirs());
                await Task.Run(() => Create.ConfigFiles());               
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                MessageBox.Show("Unable to initialize startup! See error.log\nPress 'OK' to close the app.", "Error");
                Environment.Exit(0);
            }

            bool hasErrorOccurredOnLoad = SettingsHandler.Load();

            if (hasErrorOccurredOnLoad)
            {
                MessageBox.Show("Unable to load settings.config! See error.log\nPress 'OK' to close the app.", "Error");

                Environment.Exit(0);
            }

            if (!File.Exists(SharedSettings.PathToPython))
            {
                MessageBox.Show("Unable to locate Python 2.7 installation!\nPlease select pythonw.exe...", "Error");

                string path = Python.ChangePath();

                if (path == String.Empty)
                {
                    MessageBox.Show("Unable to locate pythonw.exe!\nPress 'OK' to close the app.", "Error");

                    Environment.Exit(0);
                }
                else
                {                    
                    bool isCorrectPythonVersion = Python.CheckVersion();

                    if (!isCorrectPythonVersion)
                    {
                        MessageBox.Show("Incorrect version of Python detected!\nIt must be version 2.7!\nPress 'OK' to close the app.", "Error");

                        Environment.Exit(0);
                    }                    
                    

                    bool hasErrorOccurredOnSave = SettingsHandler.Save();

                    if (hasErrorOccurredOnSave)
                    {
                        MessageBox.Show("Unable to save settings.config! See error.log\nPress 'OK' to close the app.", "Error");

                        Environment.Exit(0);
                    }
                }
            }           

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

            if (Settings.IsAutoUpdateCheckEnabled)
                Check.Update();

            splashscreen.Close();
            WindowState = WindowState.Normal;

            //On some systems the window will be stuck behind other windows on startup
            //Workaround: This will hopefully bring the window to the foreground on different OS's 
            Show();
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();           
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

