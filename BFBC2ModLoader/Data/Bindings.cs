

namespace BFBC2ModLoader.Data
{
    public class Bindings
    {

    }

    public class ModManagerItem
    {
        public string ModOrder { get; set; }
        public bool ModEnabled { get; set; }
        public string ModName { get; set; }
        public string ModVersion { get; set; }
        public string ModAuthor { get; set; }
        public string ModType { get; set; }
        public string ModMapID { get; set; }
        public string ModImage { get; set; }
        public string ModLink { get; set; }
    }

    public class ServerBrowserItem
    {
        public string ServerName { get; set; }
        public string ServerMap { get; set; }
        public string ServerMode { get; set; }
        public string ServerBackend { get; set; }
        public string ServerPlayers { get; set; }
        public string ServerPing { get; set; }
        public string ServerReq { get; set; }
    }

    public class MapBrowserItem
    {
        public string MapName { get; set; }
        public string MapVersion { get; set; }
        public string MapSize { get; set; }
        public string MapAuthor { get; set; }
        public string MapLink { get; set; }
        public string MapImage { get; set; }
        public string MapReq { get; set; }
    }
}
