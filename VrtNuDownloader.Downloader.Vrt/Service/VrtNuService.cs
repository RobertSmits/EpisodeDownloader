using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using VrtNuDownloader.Core.Service.Logging;
using VrtNuDownloader.Downloader.Vrt.Models.Api;

namespace VrtNuDownloader.Downloader.Vrt.Service
{
    public class VrtNuService : IVrtNuService
    {
        private readonly ILoggingService _logService;
        private readonly IVrtTokenService _vrtTokenService;

        public VrtNuService
            (
                ILoggingService logService,
                IVrtTokenService vrtTokenService
            )
        {
            _logService = logService;
            _vrtTokenService = vrtTokenService;
        }

        public Uri[] GetShowSeasons(Uri showUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument html = web.Load(showUrl);
            var seasonSelectOIptions = html.DocumentNode.SelectNodes("//*[@class=\"vrt-labelnav\"]")
                ?.FirstOrDefault()
                ?.SelectNodes(".//li//a");

            if ((seasonSelectOIptions?.Count ?? 0) <= 1)
                return new Uri[] { showUrl };

            return seasonSelectOIptions
                .Select(x => new Uri("https://www.vrt.be" + x.GetAttributeValue("href", "")))
                .OrderBy(x => x.AbsoluteUri)
                .ToArray();
        }

        public Uri[] GetShowSeasonEpisodes(Uri seasonUrl)
        {
            HtmlDocument html = new HtmlWeb().Load(seasonUrl);
            var seasonEpisodes = html.DocumentNode.SelectSingleNode("//ul[@aria-labelledby='episodelist-title']")
                ?.SelectNodes(".//li//a")
                ?.Select(x => new Uri("https://www.vrt.be" + x.GetAttributeValue("href", "")))
                .OrderBy(x => x.AbsoluteUri)
                .ToArray();
            return seasonEpisodes ?? (html.DocumentNode.SelectSingleNode("//div[@class='vrtvideo']") != null ? new Uri[] { seasonUrl } : null);
        }

        public VrtContent GetEpisodeInfo(Uri episodeUrl)
        {
            var episodeURL = episodeUrl.AbsoluteUri;
            var contentJsonUrl = episodeURL.Remove(episodeURL.Length - 1) + ".content.json";
            var contentJson = new WebClient().DownloadString(contentJsonUrl);
            return JsonConvert.DeserializeObject<VrtContent>(contentJson);
        }

        public VrtPbsPub GetPublishInfo(string publicationId, string videoId)
        {
            var pbsPubURL = $"https://mediazone.vrt.be/api/v1/vrtvideo/assets/{publicationId}${videoId}";
            var pbsPubJson = new WebClient().DownloadString(pbsPubURL);
            return JsonConvert.DeserializeObject<VrtPbsPub>(pbsPubJson);
        }

        public VrtContent GetEpisodeInfoV2(Uri episodeUrl)
        {
            var epInfo = new VrtContent();
            try
            {
                epInfo = GetEpisodeInfo(episodeUrl);
            }
            catch
            {
                _logService.WriteLog(MessageType.Error, "Old episode info failed");
            }
            finally
            {
                HtmlDocument html = new HtmlWeb().Load(episodeUrl);
                var div = html.DocumentNode.SelectSingleNode("//div[@class='vrtvideo']");
                epInfo.publicationId = div.GetAttributeValue("data-publicationid", "");
                epInfo.videoId = div.GetAttributeValue("data-videoid", "");
            }
            return epInfo;
        }

        public VrtPbsPubv2 GetPublishInfoV2(string publicationId, string videoId)
        {
            var pbsPubURL = $"https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/videos/{publicationId}${videoId}?vrtPlayerToken={_vrtTokenService.PlayerToken}&client=vrtvideo";
            var pbsPubJson = new WebClient().DownloadString(pbsPubURL);
            return JsonConvert.DeserializeObject<VrtPbsPubv2>(pbsPubJson);
        }
    }
}
