using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BFBC2Toolkit.Data
{
    public class CustomTreeViewItem : INotifyPropertyChanged
    {
        public CustomTreeViewItem()
        {
            this.Items = new ObservableCollection<CustomTreeViewItem>();
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Supported { get; set; }
        public string Path { get; set; }
        public string Archive { get; set; }
        public string TreeViewPath { get; set; }
        public CustomTreeViewItem ParentItem { get; set; }

        public ObservableCollection<CustomTreeViewItem> Items { get; set; }

        private static object _selectedItem = null;

        // This is public get-only here but you could implement a public setter which
        // also selects the item.
        // Also this should be moved to an instance property on a VM for the whole tree, 
        // otherwise there will be conflicts for more than one tree.
        public static object SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged();
                }
            }
        }

        public static void OnSelectedItemChanged()
        {
            // Raise event / do other things
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    if (_isSelected)
                    {
                        SelectedItem = this;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
