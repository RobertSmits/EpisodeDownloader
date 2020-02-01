using System;
using System.Linq;
using System.Net;
using EpisodeDownloader.Downloader.Vrt.Models.Api;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EpisodeDownloader.Downloader.Vrt.Service
{
    public class VrtNuService : IVrtNuService
    {
        private readonly ILogger _logger;
        private readonly IVrtTokenService _vrtTokenService;

        public VrtNuService
            (
                ILogger<VrtNuService> logger,
                IVrtTokenService vrtTokenService
            )
        {
            _logger = logger;
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
            var seasonEpisodes = html.DocumentNode.SelectSingleNode("//nui-list[@id='episodelist-list']//nui-list--content")
                ?.SelectNodes(".//li//nui-tile")
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

        public VrtPbsPubV2 GetPublishInfo(string publicationId, string videoId)
        {
            var pbsPubURL = $"https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/videos/{publicationId}${videoId}?vrtPlayerToken={_vrtTokenService.PlayerToken}&client=vrtvideo";
            var pbsPubJson = new WebClient().DownloadString(pbsPubURL);
            return JsonConvert.DeserializeObject<VrtPbsPubV2>(pbsPubJson);
        }
    }
}
