using System;

namespace EpisodeDownloader.Contracts
{
    public class EpisodeInfo
    {
        public string ShowName { get; set; }
        public string Season { get; set; }
        public int Episode { get; set; }
        public string Title { get; set; }
        public Uri StreamUrl { get; set; }

        public TimeSpan Skip { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
