using System.Collections.Generic;

namespace VrtNuDownloader.Models
{
    public  class VrtPbsPub
    {
        public string title { get; set; }
        public string description { get; set; }
        public int duration { get; set; }
        public string aspectRatio { get; set; }
        public MetaInfo metaInfo { get; set; }
        public List<TargetUrl> targetUrls { get; set; }
        public List<object> subtitleUrls { get; set; }
        public string posterImageUrl { get; set; }
        public List<object> tags { get; set; }
    }
}
