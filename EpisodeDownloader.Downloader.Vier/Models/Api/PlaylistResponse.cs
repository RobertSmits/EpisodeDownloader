namespace EpisodeDownloader.Downloader.Vier.Models.Api;

public class PlaylistResponse
{
    public String Image { get; set; }
    public Number Length { get; set; }
    public String Description { get; set; }
    public String PublishedOn { get; set; }
    public String Label { get; set; }
    public String Video { get; set; }
    public String Uuid { get; set; }
    public String Url { get; set; }
}

public class Number
{
    public string N { get; set; }
}

public class String
{
    public string S { get; set; }
}
