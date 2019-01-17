namespace EpisodeDownloader.Downloader.Vier.Models.Api
{
    public class PlaylistResponse
    {
        public Length length { get; set; }
        public Video video { get; set; }
    }

    public class Length
    {
        public int N { get; set; }
    }

    public class Video
    {
        public string S { get; set; }
    }
}
