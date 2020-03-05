using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFBC2_Toolkit.Data
{
    public class MediaStream
    {
        public static Stream stream;

        public static async Task Dispose()
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
                await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));
            }
        }
    }
}
