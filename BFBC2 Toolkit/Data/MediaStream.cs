using System;
using System.IO;
using System.Threading.Tasks;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Data
{
    public class MediaStream
    {
        public static Stream stream;

        public static async Task Dispose()
        {
            try
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                    await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Ops, something went wrong! See error.log", "error");
            }
        }
    }
}
