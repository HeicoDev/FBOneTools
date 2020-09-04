using System;
using System.IO;
using System.Threading.Tasks;
using BFBC2Shared.Functions;

namespace BFBC2Toolkit.Helpers
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
                Log.Error(ex.ToString());
                Log.Write("Ops, something went wrong! See error.log", "error");
            }
        }
    }
}
