using System;
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
using MahApps.Metro.Controls;

namespace BFBC2_Toolkit.Windows
{
    public partial class FilePorterWindow : MetroWindow
    {
        public FilePorterWindow()
        {
            InitializeComponent();
        }

        private void TxtBoxDragAndDrop_PreviewDragOver(object sender, DragEventArgs e)
        {
            //Allow drag and drop handler of the textbox to handle all file formats
            e.Handled = true;
        }

        private void TxtBoxDragAndDrop_Drop(object sender, DragEventArgs e)
        {

        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This tool does not do anything yet.", "Info (Placeholder)");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
