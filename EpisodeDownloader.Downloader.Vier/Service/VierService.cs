﻿using HtmlAgilityPack;
using System;
using System.Net;
using Newtonsoft.Json;
using EpisodeDownloader.Core.Models;
using EpisodeDownloader.Downloader.Vier.Models.Api;
using System.Linq;

namespace EpisodeDownloader.Downloader.Vier.Service
{
    public class VierService : IVierService
    {
        private readonly IVierAuthService _vierAuthService;

        public VierService(IVierAuthService vierAuthService)
        {
            _vierAuthService = vierAuthService;
        }

        public Uri[] GetShowSeasonEpisodes(Uri seasonUrl)
        {
            var html = new HtmlWeb().Load(seasonUrl);
            var playListItems = html.DocumentNode.SelectNodes("//*[@class=\"playlist__items\"]")
                .FirstOrDefault()
                ?.SelectNodes(".//a");
            if (playListItems == null)
                return new Uri[]{};

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
            var episodeId = GetEpisodeId(html);

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

        public string GetEpisodeId(Uri episodeUrl)
        {
            var html = new HtmlWeb().Load(episodeUrl);
            return GetEpisodeId(html);
        }

        private string GetEpisodeId(HtmlDocument html)
        {
            var videoDiv = html.DocumentNode.SelectNodes("//*[contains(@class,'video-container')]").FirstOrDefault();
            return videoDiv.GetAttributeValue("id", "");
        }

        public Uri GetStreamUrl(string episodeId)
        {
            var url = $"https://api.viervijfzes.be/content/{episodeId}";
            var webClient = new WebClient();
            webClient.Headers.Add("Authorization", _vierAuthService.IdToken);
            var resultJson = webClient.DownloadString(url);
            return new Uri(JsonConvert.DeserializeObject<PlaylistResponse>(resultJson).video.S);
        }
    }
}