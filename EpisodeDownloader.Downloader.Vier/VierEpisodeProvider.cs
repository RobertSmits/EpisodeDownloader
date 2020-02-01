using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using EpisodeDownloader.Contracts;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Downloader.Vier.Models.Api;
using EpisodeDownloader.Downloader.Vier.Service;
using HtmlAgilityPack;

namespace EpisodeDownloader.Downloader.Vier
{
    public class VierEpisodeProvider : IEpisodeProvider
    {
        private readonly IVierAuthService _vierAuthService;

        public VierEpisodeProvider(IVierAuthService vierAuthService)
        {
            _vierAuthService = vierAuthService;
        }

        public bool CanHandleUrl(Uri showUrl)
        {
            return showUrl.AbsoluteUri.Contains("vier.be")
                || showUrl.AbsoluteUri.Contains("vijf.be")
                || showUrl.AbsoluteUri.Contains("zestv.be");
        }

        public Uri[] GetShowSeasons(Uri showUrl)
        {
            return new Uri[] { showUrl };
        }

        public Uri[] GetShowSeasonEpisodes(Uri seasonUrl)
        {
            var html = new HtmlWeb().Load(seasonUrl);
            var playListItems = html.DocumentNode.SelectNodes("//*[@class=\"playlist__items\"]")
                .FirstOrDefault()
                ?.SelectNodes(".//a");
            if (playListItems == null)
                return new Uri[] { };

            return playListItems
                .Where(x =>
                    x.SelectNodes(".//*[@class=\"video-teaser__title\"]//span")
                        ?.FirstOrDefault()
                        .InnerText
                        .ContainsAny(" - S", " - Aflevering") == true
                ).Select(x => new Uri(seasonUrl.Scheme + "://" + seasonUrl.Host + x.GetAttributeValue("href", "")))
                .ToArray();
        }

        public EpisodeInfo GetEpisodeInfo(Uri episodeUrl)
        {

            var html = new HtmlWeb().Load(episodeUrl);
            var title = html.DocumentNode.SelectNodes("//*[contains(@class,'metadata__title')]").FirstOrDefault().InnerText;
            var titleParts = title.Split(" - ");
            var episodeId = html.DocumentNode.SelectNodes("//*[contains(@class,'video-container')]").FirstOrDefault().GetAttributeValue("id", ""); ; ;

            return titleParts.Count() == 2
                ? new EpisodeInfo
                {
                    ShowName = titleParts[0],
                    Season = "1",
                    Episode = int.Parse(titleParts[1].Replace("Aflevering ", "")),
                    Title = "",
                    StreamUrl = GetStreamUrl(episodeId),
                }
                : new EpisodeInfo
                {
                    ShowName = titleParts[0],
                    Season = titleParts[1].Replace("S", ""),
                    Episode = int.Parse(titleParts[2].Replace("Aflevering ", "")),
                    Title = "",
                    StreamUrl = GetStreamUrl(episodeId),
                };
        }

        private Uri GetStreamUrl(string episodeId)
        {
            var url = $"https://api.viervijfzes.be/content/{episodeId}";
            var webClient = new WebClient();
            webClient.Headers.Add("Authorization", _vierAuthService.IdToken);
            var resultJson = webClient.DownloadString(url);
            return new Uri(JsonSerializer.Deserialize<PlaylistResponse>(resultJson).video.S);
        }
    }
}
