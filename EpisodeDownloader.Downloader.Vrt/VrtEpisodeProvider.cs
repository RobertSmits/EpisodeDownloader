using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EpisodeDownloader.Contracts;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Contracts.Exceptions;
using EpisodeDownloader.Downloader.Vrt.Extensions;
using EpisodeDownloader.Downloader.Vrt.Models.Api;
using EpisodeDownloader.Downloader.Vrt.Service;
using Newtonsoft.Json.Linq;

namespace EpisodeDownloader.Downloader.Vrt;

public class VrtEpisodeProvider : IEpisodeProvider
{
    private readonly HttpClient _httpClient;
    private readonly IVrtTokenService _vrtTokenService;

    public VrtEpisodeProvider(IVrtTokenService vrtTokenService)
    {
        _vrtTokenService = vrtTokenService;
        _httpClient = new HttpClient();
    }

    public bool CanHandleUrl(Uri showUrl)
    {
        return showUrl.AbsoluteUri.Contains("vrt.be");
    }

    public async Task<Uri[]> GetShowSeasonsAsync(Uri showUrl, CancellationToken cancellationToken = default)
    {
        var programName = showUrl.OriginalString.Split('/', StringSplitOptions.RemoveEmptyEntries).Last().Replace(".relevant", "");
        var uri = new Uri($"https://www.vrt.be/vrtnu/a-z/{programName}.model.json");

        var searchResponse = await _httpClient.GetStringAsync(uri);
        var json = JObject.Parse(searchResponse);
        if (json["details"]["data"]["program"]["seasons"] is null)
            return Array.Empty<Uri>();

        return json["details"]["data"]["program"]["seasons"].Select(x => new Uri("https://www.vrt.be" + x["reference"]["modelUri"].ToString())).ToArray();
    }

    public async Task<Uri[]> GetShowSeasonEpisodesAsync(Uri seasonUrl, CancellationToken cancellationToken = default)
    {
        var searchResponse = await _httpClient.GetStringAsync(seasonUrl);
        var json = JObject.Parse(searchResponse);
        return json[":items"]["par"][":items"]["container"][":items"]["list"][":items"].Values().Select(x => new Uri("https://www.vrt.be" + x["reference"]["modelUri"].ToString())).ToArray();
    }

    public async Task<EpisodeInfo> GetEpisodeInfoAsync(Uri episodeUrl, CancellationToken cancellationToken = default)
    {
        var searchResponse = await _httpClient.GetStringAsync(episodeUrl);
        var epInfo = System.Text.Json.JsonSerializer.Deserialize<SearchResponse>(searchResponse);

        var videoId = epInfo.Details.Data.Episode.VideoId;
        var publicationId = epInfo.Details.Data.Episode.PublicationId;

        var pbsPubURL = new Uri($"https://media-services-public.vrt.be/media-aggregator/v2/media-items/{publicationId}%24{videoId}")
            .AddParameter("vrtPlayerToken", await _vrtTokenService.GetPlayerTokenAsync())
            .AddParameter("client", "vrtnu-web@PROD");

        var pbsPubJson = await _httpClient.GetStringAsync(pbsPubURL);
        var pubInfo = System.Text.Json.JsonSerializer.Deserialize<VrtPbsPubV2>(pbsPubJson);
        if (pubInfo.Drm is not null)
            throw new DownloadException("Episode has DRM protection!");

        var dashUrl = pubInfo.TargetUrls.FirstOrDefault(x => x.Type.ToLower() == "mpeg_dash")?.Url;
        var hlsUrl = pubInfo.TargetUrls.FirstOrDefault(x => x.Type.ToLower() == "hls")?.Url;
        var episodeDownloadUrl = dashUrl ?? hlsUrl;

        if (episodeDownloadUrl == null)
            throw new DownloadException("Couldn't find a valid dowload playlist");

        return new EpisodeInfo
        {
            ShowName = epInfo.Details.Data.Program.Title,
            Season = epInfo.Details.Data.Season.Name,
            Episode = epInfo.Details.Data.Episode.Number.Raw,
            Title = epInfo.Details.Title,
            StreamUrl = new Uri(episodeDownloadUrl),
            Skip = TimeSpan.FromSeconds(pubInfo.Playlist.Content.TakeWhile(x => x.EventType != "STANDARD").Sum(x => x.Duration) / 1000),
            Duration = TimeSpan.FromSeconds((pubInfo.Playlist.Content.FirstOrDefault(x => x.EventType == "STANDARD")?.Duration ?? 0) / 1000),
        };
    }
}
