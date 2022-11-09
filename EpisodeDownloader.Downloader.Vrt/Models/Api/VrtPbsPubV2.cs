using System.Text.Json.Serialization;

namespace EpisodeDownloader.Downloader.Vrt.Models.Api
{
    public class VrtPbsPubV2
    {
        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("skinType")]
        public string SkinType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("drm")]
        public object Drm { get; set; }

        [JsonPropertyName("drmExpired")]
        public object DrmExpired { get; set; }

        [JsonPropertyName("aspectRatio")]
        public string AspectRatio { get; set; }

        [JsonPropertyName("targetUrls")]
        public TargetUrl[] TargetUrls { get; set; }

        [JsonPropertyName("posterImageUrl")]
        public string PosterImageUrl { get; set; }

        [JsonPropertyName("channelId")]
        public object ChannelId { get; set; }

        [JsonPropertyName("playlist")]
        public Playlist Playlist { get; set; }

        [JsonPropertyName("chaptering")]
        public Chaptering Chaptering { get; set; }
    }

    public class TargetUrl
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class Playlist
    {
        [JsonPropertyName("content")]
        public Content[] Content { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("skippable")]
        public bool Skippable { get; set; }
    }

    public class Chaptering
    {
        [JsonPropertyName("content")]
        public object[] Content { get; set; }
    }
}
