namespace EpisodeDownloader.Downloader.Vier.Models.Api
{
    public class PlaylistResponse
    {
        public String image { get; set; }
        public Number length { get; set; }
        public String description { get; set; }
        public String publishedOn { get; set; }
        public String label { get; set; }
        public String video { get; set; }
        public String uuid { get; set; }
        public String url { get; set; }
    }

    public class Number
    {
        public string N { get; set; }
    }

    public class String
    {
        public string S { get; set; }
    }
}
