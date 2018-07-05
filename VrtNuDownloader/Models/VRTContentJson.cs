using System.Collections.Generic;

namespace VrtNuDownloader.Models
{
    class VRTContentJson
    {
        public string title { get; set; }
        public string name { get; set; }
        public string pagePath { get; set; }
        public string description { get; set; }
        public string shortDescription { get; set; }
        public string broadcastDate { get; set; }
        public string broadcastFullDate { get; set; }
        public string broadcastShortDate { get; set; }
        public int episodeNumber { get; set; }
        public string seasonTitle { get; set; }
        public string seasonName { get; set; }
        public string seasonPath { get; set; }
        public string whatsonId { get; set; }
        public int duration { get; set; }
        public string videoId { get; set; }
        public string assetStatus { get; set; }
        public string transcodingStatus { get; set; }
        public string assetImage { get; set; }
        public string assetImageId { get; set; }
        public int assetOnTime { get; set; }
        public int assetOffTime { get; set; }
        public string genre { get; set; }
        public string programName { get; set; }
        public string programTitle { get; set; }
        public string programPath { get; set; }
        public string programType { get; set; }
        public string programUrl { get; set; }
        public string programImageUrl { get; set; }
        public string programDescription { get; set; }
        public int lastReplicationDate { get; set; }
        public string url { get; set; }
        public List<string> tags { get; set; }
        public List<string> programTags { get; set; }
        public List<object> chapters { get; set; }
        public string publicationId { get; set; }
        public string allowedRegion { get; set; }
    }
}
