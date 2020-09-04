

namespace BFBC2ModLoader.Data
{
    public class Globals
    {
        public static string VersionClient { get; } = "2.0.1";
        public static string VersionClientNew { get; set; }
        public static string VersionServer { get; set; }
           
        public static bool MapUpdatesAvailable { get; set; } = false;
        public static bool IsClient { get; set; } = true;
        public static bool HasModMoved { get; set; } = false;
        public static bool HasModBeenUnChecked { get; set; } = false;
    }
}
