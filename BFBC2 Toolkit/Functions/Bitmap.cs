using System.IO;
using System.Windows.Media.Imaging;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Bitmap
    {
        public static BitmapImage LoadImage(string filePath)
        {
            MediaStream.stream = new FileStream(filePath, FileMode.Open);

            var bitmap = new BitmapImage();            
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.StreamSource = MediaStream.stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
    }
}
