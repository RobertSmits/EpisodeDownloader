using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EpisodeDownloader.Contracts;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Contracts.Exceptions;
using EpisodeDownloader.Downloader.Vrt.Extensions;
using EpisodeDownloader.Downloader.Vrt.Models.Api;
using EpisodeDownloader.Downloader.Vrt.Service;

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

        public Task<Uri[]> GetShowSeasonsAsync(Uri showUrl, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new Uri[] { showUrl });
        }

        public async Task<Uri[]> GetShowSeasonEpisodesAsync(Uri seasonUrl, CancellationToken cancellationToken = default)
        {
            var searchUrl = "//" + seasonUrl.Host + seasonUrl.AbsolutePath.Replace(".relevant", "");
            var uri = new Uri("https://search.vrt.be/search?size=150&facets%5BprogramUrl%5D=" + searchUrl);
            var searchResponse = await new WebClient().DownloadStringTaskAsync(uri);
            var searchResults = JsonSerializer.Deserialize<SearchResponse>(searchResponse);
            return searchResults.results.Select(x => new Uri(seasonUrl.Scheme + ":" + x.url)).ToArray();
        }

        public async Task<EpisodeInfo> GetEpisodeInfoAsync(Uri episodeUrl, CancellationToken cancellationToken = default)
        {
            var episodeURL = episodeUrl.AbsoluteUri;
            var contentJsonUrl = episodeURL.Remove(episodeURL.Length - 1) + ".content.json";
            var contentJson = await new WebClient().DownloadStringTaskAsync(contentJsonUrl);
            var episodeInfo = JsonSerializer.Deserialize<VrtContent>(contentJson);

            var pbsPubURL = new Uri($"https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/videos/{episodeInfo.publicationId}${episodeInfo.videoId}")
                .AddParameter("vrtPlayerToken", await _vrtTokenService.GetPlayerTokenAsync())
                .AddParameter("client", "vrtvideo");

            var pbsPubJson = await new WebClient().DownloadStringTaskAsync(pbsPubURL);
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
                Skip = TimeSpan.FromSeconds(pubInfo.playlist.content.TakeWhile(x => x.eventType != "STANDARD").Sum(x => x.duration) / 1000),
                Duration = TimeSpan.FromSeconds((pubInfo.playlist.content.FirstOrDefault(x => x.eventType == "STANDARD")?.duration ?? 0) / 1000),
            };
        }
    }
}
