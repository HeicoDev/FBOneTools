using BFBC2Toolkit.Data;

namespace BFBC2Toolkit.Functions
{
    public class Write
    {
        public static void ToInfoBox(CustomTreeViewItem ctvi)
        {
            switch (ctvi.Type)
            {
                case "Texture":
                    UIElements.TxtBoxInformation.Document.Blocks.Clear();
                    UIElements.TxtBoxInformation.Document.PageWidth = ctvi.Name.Length * 15;
                    UIElements.TxtBoxInformation.AppendText($"Name:\r{ ctvi.Name }\r\rType:\r{ ctvi.Type }\r\rFormat:\r{ ctvi.Format }\r{ Globals.SelectedTexture.Format }\r\rResolution:\r{ Globals.SelectedTexture.Width }x{ Globals.SelectedTexture.Height }\r\rMipmaps:\r{ Globals.SelectedTexture.MipmapCount }\r\rArchive:\r{ ctvi.Archive }\r\rSupported:\r{ ctvi.Supported }");
                    break;
                case "Heightmap":
                    UIElements.TxtBoxInformation.Document.Blocks.Clear();
                    UIElements.TxtBoxInformation.Document.PageWidth = ctvi.Name.Length * 15;
                    UIElements.TxtBoxInformation.AppendText($"Name:\r{ ctvi.Name }\r\rType:\r{ ctvi.Type }\r\rResolution:\r{ Globals.SelectedTexture.Width }x{ Globals.SelectedTexture.Height }\r\rArchive:\r{ ctvi.Archive }\r\rSupported:\r{ ctvi.Supported }");
                    break;
                default:
                    UIElements.TxtBoxInformation.Document.Blocks.Clear();
                    UIElements.TxtBoxInformation.Document.PageWidth = ctvi.Name.Length * 15;
                    UIElements.TxtBoxInformation.AppendText($"Name:\r{ ctvi.Name }\r\rType:\r{ ctvi.Type }\r\rFormat:\r{ ctvi.Format }\r\rArchive:\r{ ctvi.Archive }\r\rSupported:\r{ ctvi.Supported }");
                    break;
            }     
        }
    }
}
