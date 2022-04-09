using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace BFBC2Toolkit.Data
{
    public class UIElements
    {
        public static RichTextBox TxtBoxEventLog { get; set; }
        public static RichTextBox TxtBoxInformation { get; set; }
        public static TextEditor TextEditor { get; set; }
        public static TreeView TreeViewDataExplorer { get; set; }
        public static TreeView TreeViewModExplorer { get; set; }
        public static MediaElement MediaElement { get; set; }
        public static Image ImageElement { get; set; }
        public static CompletionWindow CodeComWindow { get; set; }
        public static Grid GridPreviewLogProp { get; set; }
        public static Grid GridPreviewProp { get; set; }
    }
}
