using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BFBC2Toolkit.Data;

namespace BFBC2Toolkit.Helpers
{
    public class BitmapHelper
    {
        public static BitmapImage LoadImage(string filePath)
        {
            MediaStream.CurrentStream = new FileStream(filePath, FileMode.Open);

            var bitmap = new BitmapImage();            
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.StreamSource = MediaStream.CurrentStream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        public static BitmapSource LoadGrayscaleImage(string filePath)
        {
            int stride = (Globals.SelectedTexture.Width * 16 + 7) / 8;

            byte[] fileData = { };

            using (var br = new BinaryReader(File.OpenRead(filePath)))
            {
                fileData = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
            }

            var bitmap = BitmapSource.Create(Globals.SelectedTexture.Width, Globals.SelectedTexture.Height, 96, 96, PixelFormats.Gray16, null, fileData, stride);

            return bitmap;
        }
    }
}
