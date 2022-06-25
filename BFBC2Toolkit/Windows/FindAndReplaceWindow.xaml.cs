using System;
using System.Text.RegularExpressions;
using System.Media;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using BFBC2Toolkit.Data;
using BFBC2Toolkit.Functions;

namespace BFBC2Toolkit.Windows
{
    public partial class FindAndReplaceWindow : MetroWindow
    {
        private static string textToFind = "";

        private static bool caseSensitive = false,
                            wholeWord = false,
                            useRegex = false,
                            useWildcards = false,
                            searchUp = false;

        private TextEditor editor;

        private static ObservableCollection<string> searchHistoryItems = new ObservableCollection<string>();
        private static ObservableCollection<string> replaceHistoryItems = new ObservableCollection<string>();

        public FindAndReplaceWindow(TextEditor editor)
        {
            InitializeComponent();

            this.editor = editor;

            Owner = GetWindow(Application.Current.MainWindow);           

            txtFind.Text = txtFind2.Text = textToFind;
            cbCaseSensitive.IsChecked = caseSensitive;
            cbWholeWord.IsChecked = wholeWord;
            cbRegex.IsChecked = useRegex;
            cbWildcards.IsChecked = useWildcards;
            cbSearchUp.IsChecked = searchUp;
            txtFind.ItemsSource = searchHistoryItems;
            txtFind2.ItemsSource = searchHistoryItems;
            txtReplace.ItemsSource = replaceHistoryItems;            
        }       

        private void FindAndReplaceWindow_Closed(object sender, EventArgs e)
        {
            textToFind = txtFind2.Text;
            caseSensitive = (cbCaseSensitive.IsChecked == true);
            wholeWord = (cbWholeWord.IsChecked == true);
            useRegex = (cbRegex.IsChecked == true);
            useWildcards = (cbWildcards.IsChecked == true);
            searchUp = (cbSearchUp.IsChecked == true);

            theDialog = null;
        }

        private void FindAndReplaceWindow_Activated(object sender, EventArgs e)
        {
            IsTransparent(false);
        }

        private void FindAndReplaceWindow_Deactivated(object sender, EventArgs e)
        {
            IsTransparent(true);
        }

        private async void FindAndReplaceWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (UIElements.TextEditor.Visibility == Visibility.Visible)
            {
                if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.KeyboardDevice.IsKeyDown(Key.S))
                    await Save.TextEditorChanges();
            }
        }

        private void IsTransparent(bool isTransparent)
        {
            byte transparency = 255;

            if (isTransparent)
                transparency = 100;

            Background = new SolidColorBrush(Color.FromArgb(transparency, 41, 41, 41));
            //Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            //WindowTitleBrush = new SolidColorBrush(Color.FromArgb(transparency, 31, 31, 31));
            //BorderBrush = new SolidColorBrush(Color.FromArgb(transparency, 31, 31, 31));
            tabMain.Background = new SolidColorBrush(Color.FromArgb(transparency, 41, 41, 41));
            lblFind.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            lblFind2.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            lblReplace.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            txtFind.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            txtFind.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            txtFind2.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            txtFind2.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            txtReplace.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            txtReplace.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            btnFindNext.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            btnFindNext.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            btnFindNext2.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            btnFindNext2.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            btnFindPrev.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            btnFindPrev.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            btnFindPrev2.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            btnFindPrev2.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            btnReplace.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            btnReplace.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            btnReplaceAll.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            btnReplaceAll.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            cbCaseSensitive.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            cbCaseSensitive.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            cbWholeWord.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            cbWholeWord.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            cbRegex.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            cbRegex.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            cbWildcards.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            cbWildcards.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
            cbSearchUp.Background = new SolidColorBrush(Color.FromArgb(transparency, 69, 69, 69));
            cbSearchUp.Foreground = new SolidColorBrush(Color.FromArgb(transparency, 226, 226, 226));
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            cbSearchUp.IsChecked = false;

            if (!FindNext(txtFind.Text))
                SystemSounds.Beep.Play();
        }

        private void FindNext2Click(object sender, RoutedEventArgs e)
        {
            cbSearchUp.IsChecked = false;

            if (!FindNext(txtFind2.Text))
                SystemSounds.Beep.Play();
        }

        private void FindPrevClick(object sender, RoutedEventArgs e)
        {
            cbSearchUp.IsChecked = true;

            if (!FindNext(txtFind.Text))
                SystemSounds.Beep.Play();
        }

        private void FindPrev2Click(object sender, RoutedEventArgs e)
        {
            cbSearchUp.IsChecked = true;

            if (!FindNext(txtFind2.Text))
                SystemSounds.Beep.Play();
        }

        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtReplace.Text))
                UpdateReplaceHistory();

            Regex regex = GetRegEx(txtFind2.Text);
            string input = editor.Text.Substring(editor.SelectionStart, editor.SelectionLength);
            Match match = regex.Match(input);
            bool replaced = false;
            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                editor.Document.Replace(editor.SelectionStart, editor.SelectionLength, txtReplace.Text);
                replaced = true;
            }

            if (!FindNext(txtFind2.Text) && !replaced)
                SystemSounds.Beep.Play();
        }

        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtReplace.Text))
                UpdateReplaceHistory();

            if (MessageBox.Show("Are you sure you want to Replace All occurences of \"" +
            txtFind2.Text + "\" with \"" + txtReplace.Text + "\"?",
                "Replace All", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Regex regex = GetRegEx(txtFind2.Text, true);
                int offset = 0;
                editor.BeginChange();
                foreach (Match match in regex.Matches(editor.Text))
                {
                    editor.Document.Replace(offset + match.Index, match.Length, txtReplace.Text);
                    offset += txtReplace.Text.Length - match.Length;
                }
                editor.EndChange();
            }

            Application.Current.MainWindow.Focus(); 
        }

        private bool FindNext(string textToFind)
        {
            if (!string.IsNullOrWhiteSpace(textToFind))
                UpdateSearchHistory(textToFind);

            Regex regex = GetRegEx(textToFind);
            int start = regex.Options.HasFlag(RegexOptions.RightToLeft) ?
            editor.SelectionStart : editor.SelectionStart + editor.SelectionLength;
            Match match = regex.Match(editor.Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                if (regex.Options.HasFlag(RegexOptions.RightToLeft))
                    match = regex.Match(editor.Text, editor.Text.Length);
                else
                    match = regex.Match(editor.Text, 0);
            }

            if (match.Success)
            {
                editor.Select(match.Index, match.Length);
                TextLocation loc = editor.Document.GetLocation(match.Index);
                editor.ScrollTo(loc.Line, loc.Column);
            }

            return match.Success;
        }

        private void UpdateSearchHistory(string textToFind)
        {
            if (!searchHistoryItems.Contains(textToFind))
            {
                searchHistoryItems.Insert(0, textToFind);
            }
            else
            {
                int index = 0;

                foreach (var item in searchHistoryItems)
                    if (item.ToString() == textToFind)
                        index = searchHistoryItems.IndexOf(item);

                if (index != 0)
                {
                    searchHistoryItems.RemoveAt(index);
                    searchHistoryItems.Insert(0, textToFind);
                }
            }

            txtFind.ItemsSource = searchHistoryItems;
            txtFind.Text = textToFind;
            txtFind.SelectedIndex = 0;
            txtFind.Focus();
            txtFind2.ItemsSource = searchHistoryItems;
            txtFind2.Text = textToFind;
            txtFind2.SelectedIndex = 0;
            txtFind2.Focus();
        }

        private void UpdateReplaceHistory()
        {
            string text = txtReplace.Text;

            if (!replaceHistoryItems.Contains(text))
            {
                replaceHistoryItems.Insert(0, text);
            }
            else
            {
                int index = 0;

                foreach (var item in replaceHistoryItems)
                    if (item.ToString() == text)
                        index = replaceHistoryItems.IndexOf(item);

                if (index != 0)
                {
                    replaceHistoryItems.RemoveAt(index);
                    replaceHistoryItems.Insert(0, text);
                }
            }

            txtReplace.ItemsSource = replaceHistoryItems;
            txtReplace.Text = text;
            txtReplace.Focus();
        }

        private Regex GetRegEx(string textToFind, bool leftToRight = false)
        {
            RegexOptions options = RegexOptions.None;
            if (cbSearchUp.IsChecked == true && !leftToRight)
                options |= RegexOptions.RightToLeft;
            if (cbCaseSensitive.IsChecked == false)
                options |= RegexOptions.IgnoreCase;

            if (cbRegex.IsChecked == true)
            {
                return new Regex(textToFind, options);
            }
            else
            {
                string pattern = Regex.Escape(textToFind);
                if (cbWildcards.IsChecked == true)
                    pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                if (cbWholeWord.IsChecked == true)
                    pattern = "\\b" + pattern + "\\b";
                return new Regex(pattern, options);
            }
        }       

        private static FindAndReplaceWindow theDialog = null;

        public static void ShowForReplace(TextEditor editor)
        {
            if (theDialog == null)
            {
                theDialog = new FindAndReplaceWindow(editor);
                theDialog.tabMain.SelectedIndex = 1;
                theDialog.Show();
                theDialog.Activate();
            }
            else
            {
                theDialog.tabMain.SelectedIndex = 1;
                theDialog.Activate();
            }

            if (!editor.TextArea.Selection.IsMultiline)
            {
                theDialog.txtFind.Text = theDialog.txtFind2.Text = editor.TextArea.Selection.GetText();
                //theDialog.txtFind.SelectAll();
                //theDialog.txtFind2.SelectAll();
                theDialog.txtFind2.Focus();
            }
        }        

        private void txtFind_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            cbSearchUp.IsChecked = false;

            if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.Enter))
                if (!FindNext(txtFind.Text))
                    SystemSounds.Beep.Play();
        }        

        private void txtFind2_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            cbSearchUp.IsChecked = false;

            if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.Enter))
                if (!FindNext(txtFind2.Text))
                    SystemSounds.Beep.Play();
        }

        private void txtFind_Loaded(object sender, RoutedEventArgs e)
        {
            //Popup popup = FindVisualChildByName<Popup>((sender as DependencyObject), "PART_Popup");
            //Border border = FindVisualChildByName<Border>(popup.Child, "PopupBorder");
            //border.CornerRadius = new CornerRadius(2);
        }

        private void txtFind2_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = FindVisualChildByName<TextBox>((sender as DependencyObject), "PART_EditableTextBox");
            textBox.SelectionBrush = new SolidColorBrush(Color.FromRgb(182, 182, 182));
            Popup popup = FindVisualChildByName<Popup>((sender as DependencyObject), "PART_Popup");
            Border border = FindVisualChildByName<Border>(popup.Child, "PopupBorder");
            border.CornerRadius = new CornerRadius(2);
        }

        private void txtReplace_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = FindVisualChildByName<TextBox>((sender as DependencyObject), "PART_EditableTextBox");
            textBox.SelectionBrush = new SolidColorBrush(Color.FromRgb(182, 182, 182));
            Popup popup = FindVisualChildByName<Popup>((sender as DependencyObject), "PART_Popup");
            Border border = FindVisualChildByName<Border>(popup.Child, "PopupBorder");
            border.CornerRadius = new CornerRadius(2);
        }

        private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
    }
}