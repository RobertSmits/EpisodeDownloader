namespace EpisodeDownloader.Downloader.Vrt.Models.Auth
{
    public class VrtLoginPayload
    {
        public string uid { get; set; }
        public string uidsig { get; set; }
        public string ts { get; set; }
        public string email { get; set; }
    }
}
