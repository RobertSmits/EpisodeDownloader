using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EpisodeDownloader.Contracts;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Downloader.Vier.Models.Api;
using EpisodeDownloader.Downloader.Vier.Service;
using HtmlAgilityPack;
using Newtonsoft.Json;

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
                || showUrl.AbsoluteUri.Contains("zestv.be")
                || showUrl.AbsoluteUri.Contains("goplay.be");
        }

        public Task<Uri[]> GetShowSeasonsAsync(Uri showUrl, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new Uri[] {
                new Uri($"{showUrl.Scheme}://www.goplay.be{showUrl.AbsolutePath}"),
            });
        }

        public async Task<Uri[]> GetShowSeasonEpisodesAsync(Uri seasonUrl, CancellationToken cancellationToken = default)
        {
            var html = await new HtmlWeb().LoadFromWebAsync(seasonUrl, null, null, cancellationToken);
            var heroData = html.DocumentNode.SelectSingleNode("//*[@data-hero]")?.GetAttributeValue("data-hero", "{}");
            if (heroData == null) return new Uri[0];
            var hero = JsonConvert.DeserializeObject<DataHero>(HttpUtility.HtmlDecode(heroData));

            return hero.Data.Playlists.SelectMany(x => x.Episodes.Select(x =>
                new Uri(seasonUrl.Scheme + "://" + seasonUrl.Host + x.Link))).ToArray();
        }

        public async Task<EpisodeInfo> GetEpisodeInfoAsync(Uri episodeUrl, CancellationToken cancellationToken = default)
        {
            var posibleDataTags = new[] { "data-hero", "data-program-seasons" };
            var html = await new HtmlWeb().LoadFromWebAsync(episodeUrl, null, null, cancellationToken);
            string heroData = default;
            foreach(var current in posibleDataTags)
            {
                var node = html.DocumentNode.SelectSingleNode($"//*[@{current}]");
                if (node is null) continue;
                heroData = node.GetAttributeValue(current, null);
                if (heroData != null) break;
            }

            var hero = JsonConvert.DeserializeObject<DataHero>(HttpUtility.HtmlDecode(heroData ?? "{}"));
            var episode = hero.Data.Playlists.SelectMany(x => x.Episodes).First(x => x.Link == episodeUrl.AbsolutePath.TrimEnd('/'));
            var titleParts = episode.Title.Split(" - ");

            return titleParts.Count() == 2
                ? new EpisodeInfo
                {
                    ShowName = titleParts[0],
                    Season = "1",
                    Episode = int.Parse(titleParts[1].Replace("Aflevering ", "")),
                    Title = "",
                    StreamUrl = await GetStreamUrlAsync(episode.VideoUuid),
                }
                : new EpisodeInfo
                {
                    ShowName = titleParts[0],
                    Season = titleParts[1].Replace("S", ""),
                    Episode = int.Parse(titleParts[2].Replace("Aflevering ", "")),
                    Title = "",
                    StreamUrl = await GetStreamUrlAsync(episode.VideoUuid),
                };
        }

        private async Task<Uri> GetStreamUrlAsync(string episodeId)
        {
            var url = $"https://api.viervijfzes.be/content/{episodeId}";
            var webClient = new WebClient();
            webClient.Headers.Add("Authorization", await _vierAuthService.GetIdTokenAsync());
            var resultJson = await webClient.DownloadStringTaskAsync(url);
            return new Uri(JsonConvert.DeserializeObject<PlaylistResponse>(resultJson).Video.S);
        }
    }
}
