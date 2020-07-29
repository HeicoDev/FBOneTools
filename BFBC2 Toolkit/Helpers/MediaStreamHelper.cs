using System;
using System.IO;
using System.Threading.Tasks;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Helpers
{
    public class MediaStream
    {
        public static Stream CurrentStream { get; set; }

        public static async Task Dispose()
        {
            try
            {
                if (CurrentStream != null)
                {
                    CurrentStream.Close();
                    CurrentStream.Dispose();
                    CurrentStream = null;
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
