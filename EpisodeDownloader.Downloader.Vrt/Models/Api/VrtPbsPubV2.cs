namespace EpisodeDownloader.Downloader.Vrt.Models.Api
{
    public class VrtPbsPubv2
    {
        public int duration { get; set; }
        public string skinType { get; set; }
        public string title { get; set; }
        public string shortDescription { get; set; }
        public object drm { get; set; }
        public object drmExpired { get; set; }
        public string aspectRatio { get; set; }
        public TargetUrl[] targetUrls { get; set; }
        public string posterImageUrl { get; set; }
        public object channelId { get; set; }
        public Playlist playlist { get; set; }
        public Chaptering chaptering { get; set; }
    }
}
