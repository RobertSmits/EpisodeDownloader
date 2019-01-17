namespace EpisodeDownloader.Downloader.Vrt.Models.Api
{
    public class VrtPbsPub
    {
        public string title { get; set; }
        public string description { get; set; }
        public int duration { get; set; }
        public string aspectRatio { get; set; }
        public MetaInfo metaInfo { get; set; }
        public TargetUrl[] targetUrls { get; set; }
        public object[] subtitleUrls { get; set; }
        public string posterImageUrl { get; set; }
        public object[] tags { get; set; }
    }
}
