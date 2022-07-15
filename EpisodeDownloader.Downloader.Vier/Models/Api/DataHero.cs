namespace EpisodeDownloader.Downloader.Vier.Models.Api
{
    public class DataHero
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string Link { get; set; }
        public Images Images { get; set; }
        public Header Header { get; set; }
        public Pageinfo PageInfo { get; set; }
        public Playlist[] Playlists { get; set; }
        public Social Social { get; set; }
    }

    public class Images
    {
        public string Hero { get; set; }
        public string Mobile { get; set; }
        public string Poster { get; set; }
        public string Teaser { get; set; }
    }

    public class Header
    {
        public string Title { get; set; }
        public object[] Video { get; set; }
    }

    public class Pageinfo
    {
        public string Site { get; set; }
        public string Url { get; set; }
        public string NodeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Program { get; set; }
        public string ProgramId { get; set; }
        public string ProgramUuid { get; set; }
        public string ProgramKey { get; set; }
        public string[] Tags { get; set; }
        public int PublishDate { get; set; }
        public int UnpublishDate { get; set; }
        public string Author { get; set; }
        public int NotificationsScore { get; set; }
    }

    public class Social
    {
        public string Facebook { get; set; }
        public string Hashtag { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
    }

    public class Playlist
    {
        public Episode[] Episodes { get; set; }
        public string Id { get; set; }
        public string Link { get; set; }
        public Pageinfo PageInfo { get; set; }
        public string Title { get; set; }
    }

    public class Episode
    {
        public bool Autoplay { get; set; }
        public string CimTag { get; set; }
        public int CreatedDate { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public object EmbedCta { get; set; }
        public bool EnablePreroll { get; set; }
        public string EpisodeNumber { get; set; }
        public string EpisodeTitle { get; set; }
        public bool HasProductPlacement { get; set; }
        public string Image { get; set; }
        public bool IsProtected { get; set; }
        public bool IsSeekable { get; set; }
        public bool IsStreaming { get; set; }
        public string Link { get; set; }
        public float[] MidrollOffsets { get; set; }
        public Pageinfo PageInfo { get; set; }
        public string PageUuid { get; set; }
        public string ParentalRating { get; set; }
        public string Path { get; set; }
        public Program Program { get; set; }
        public string SeasonNumber { get; set; }
        public int SeekableFrom { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int? UnpublishDate { get; set; }
        public string VideoUuid { get; set; }
        public string WhatsonId { get; set; }
        public bool Needs16PlusLabel { get; set; }
        public string Badge { get; set; }
    }

    public class Program
    {
        public string Title { get; set; }
        public string Poster { get; set; }
    }
}
