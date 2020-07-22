using System.IO;
using System.Windows.Media.Imaging;

namespace BFBC2_Toolkit.Helpers
{
    public class BitmapHelper
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
