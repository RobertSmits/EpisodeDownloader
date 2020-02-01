using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using EpisodeDownloader.Contracts;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Contracts.Exceptions;
using EpisodeDownloader.Downloader.Vrt.Extensions;
using EpisodeDownloader.Downloader.Vrt.Models.Api;
using EpisodeDownloader.Downloader.Vrt.Service;
using HtmlAgilityPack;

namespace EpisodeDownloader.Downloader.Vrt
{
    public class VrtEpisodeProvider : IEpisodeProvider
    {
        private readonly IVrtTokenService _vrtTokenService;

        public VrtEpisodeProvider(IVrtTokenService vrtTokenService)
        {
            _vrtTokenService = vrtTokenService;
        }

        public bool CanHandleUrl(Uri showUrl)
        {
            return showUrl.AbsoluteUri.Contains("vrt.be");
        }

        public Uri[] GetShowSeasons(Uri showUrl)
        {
            var html = new HtmlWeb().Load(showUrl);
            var seasonSelectOptions = html.DocumentNode.SelectNodes("//*[@class=\"vrt-labelnav\"]")
                ?.FirstOrDefault()
                ?.SelectNodes(".//li//a");

            if ((seasonSelectOptions?.Count ?? 0) <= 1)
                return new Uri[] { showUrl };

            return seasonSelectOptions
                .Select(x => new Uri("https://www.vrt.be" + x.GetAttributeValue("href", "")))
                .OrderBy(x => x.AbsoluteUri)
                .ToArray();
        }

        public Uri[] GetShowSeasonEpisodes(Uri seasonUrl)
        {
            var html = new HtmlWeb().Load(seasonUrl);
            var seasonEpisodes = html.DocumentNode.SelectSingleNode("//nui-list[@id='episodelist-list']//nui-list--content")
                ?.SelectNodes(".//li//nui-tile")
                ?.Select(x => new Uri("https://www.vrt.be" + x.GetAttributeValue("href", "")))
                .OrderBy(x => x.AbsoluteUri)
                .ToArray();
            return seasonEpisodes ?? (html.DocumentNode.SelectSingleNode("//div[@class='vrtvideo']") != null ? new Uri[] { seasonUrl } : new Uri[0]);
        }

        public EpisodeInfo GetEpisodeInfo(Uri episodeUrl)
        {
            var episodeURL = episodeUrl.AbsoluteUri;
            var contentJsonUrl = episodeURL.Remove(episodeURL.Length - 1) + ".content.json";
            var contentJson = new WebClient().DownloadString(contentJsonUrl);
            var episodeInfo = JsonSerializer.Deserialize<VrtContent>(contentJson);

            var pbsPubURL = new Uri($"https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/videos/{episodeInfo.publicationId}${episodeInfo.videoId}")
                .AddParameter("vrtPlayerToken", _vrtTokenService.PlayerToken)
                .AddParameter("client", "vrtvideo");

            var pbsPubJson = new WebClient().DownloadString(pbsPubURL);
            var pubInfo = JsonSerializer.Deserialize<VrtPbsPubV2>(pbsPubJson);

            var episodeDownloadUrl = pubInfo.targetUrls.Where(x => x.type.ToLower() == "hls").Select(x => new Uri(x.url)).FirstOrDefault();
            if (episodeDownloadUrl == null)
                throw new DownloadException("Couldn't find a valid M3U8");

            return new EpisodeInfo
            {
                ShowName = episodeInfo.programTitle,
                Season = episodeInfo.seasonTitle,
                Episode = episodeInfo.episodeNumber,
                Title = episodeInfo.title,
                StreamUrl = episodeDownloadUrl,
                Skip = TimeSpan.FromSeconds(pubInfo.playlist.content.TakeWhile(x => !x.skippable).Sum(x => x.duration) / 1000),
                Duration = TimeSpan.FromSeconds((pubInfo.playlist.content.FirstOrDefault(x => x.skippable)?.duration ?? 0) / 1000),
            };
        }
    }
}
