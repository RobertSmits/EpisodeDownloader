using System.Collections.Generic;

namespace EpisodeDownloader.Core
{
    public class Configuration
    {
        public string DownloadPath { get; set; }
        public string SavePath { get; set; }
        public bool SaveShowsInFolders { get; set; }
        public bool SaveSeasonsInFolders { get; set; }
        public IEnumerable<string> WatchUrls { get; set; }
    }
}
