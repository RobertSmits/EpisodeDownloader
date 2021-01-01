using System;

namespace EpisodeDownloader.Downloader.Vrt.Models.Api
{
    public class SearchResponse
    {
        public Meta meta { get; set; }
        public Result[] results { get; set; }
        public Facets facets { get; set; }
    }

    public class Meta
    {
        public int total_results { get; set; }
        public Pages pages { get; set; }
    }

    public class Pages
    {
        public int total { get; set; }
        public int current { get; set; }
        public int size { get; set; }
    }

    public class Facets
    {
        public Facet[] facets { get; set; }
    }

    public class Facet
    {
        public string name { get; set; }
        public Bucket[] buckets { get; set; }
    }

    public class Bucket
    {
        public string key { get; set; }
        public int doc_count { get; set; }
    }

    public class Result
    {
        public string ageGroup { get; set; }
        public string allowedRegion { get; set; }
        public string assetOffTime { get; set; }
        public string assetOnTime { get; set; }
        public string assetPath { get; set; }
        public string assetStatus { get; set; }
        public string[] brands { get; set; }
        public long broadcastDate { get; set; }
        public string[] categories { get; set; }
        public object[] chapters { get; set; }
        public string description { get; set; }
        public object displayOptions { get; set; }
        public int duration { get; set; }
        public int episodeNumber { get; set; }
        public string externalPermalink { get; set; }
        public string formattedBroadcastDate { get; set; }
        public string formattedBroadcastFullDate { get; set; }
        public string formattedBroadcastShortDate { get; set; }
        public string name { get; set; }
        public object offTimeDate { get; set; }
        public object onTimeDate { get; set; }
        public string pageId { get; set; }
        public string pagePath { get; set; }
        public string path { get; set; }
        public string permalink { get; set; }
        public bool productPlacement { get; set; }
        public string program { get; set; }
        public string programAlternativeImageUrl { get; set; }
        public string[] programBrands { get; set; }
        public string programDescription { get; set; }
        public string programImageUrl { get; set; }
        public string programName { get; set; }
        public string programPath { get; set; }
        public Programtag[] programTags { get; set; }
        public string programType { get; set; }
        public string programUrl { get; set; }
        public string programWhatsonId { get; set; }
        public string publicationId { get; set; }
        public bool seasonHidden { get; set; }
        public string seasonName { get; set; }
        public int seasonNbOfEpisodes { get; set; }
        public int seasonNumber { get; set; }
        public string seasonPath { get; set; }
        public string seasonTitle { get; set; }
        public string shortDescription { get; set; }
        public string subtitle { get; set; }
        public object[] tags { get; set; }
        public string title { get; set; }
        public string transcodingStatus { get; set; }
        public string url { get; set; }
        public string videoId { get; set; }
        public string videoThumbnailUrl { get; set; }
        public string whatsonId { get; set; }
        public string instigator { get; set; }
        public string lastIndex { get; set; }
        public string id { get; set; }
        public float score { get; set; }
        public string type { get; set; }
    }


    public class Programtag
    {
        public string path { get; set; }
        public string name { get; set; }
        public string titlePath { get; set; }
        public string title { get; set; }
        public string parentTitle { get; set; }
        public string tagId { get; set; }
        public string description { get; set; }
        public string nameSpaceName { get; set; }
        public string localTagId { get; set; }
    }
}
