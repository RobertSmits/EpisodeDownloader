using System;
using System.Threading;
using System.Threading.Tasks;
using EpisodeDownloader.Contracts;
using EpisodeDownloader.Contracts.Downloader;
using Microsoft.Extensions.Logging;

namespace EpisodeDownloader.Core.Downloader;

public class DefaultDownloader : IEpisodeProvider
{
    private readonly ILogger _logger;

    public DefaultDownloader(ILogger<DefaultDownloader> logger)
    {
        _logger = logger;
    }

    public bool CanHandleUrl(Uri showUrl)
    {
        return false;
    }

    public Task<Uri[]> GetShowSeasonsAsync(Uri showUrl, CancellationToken cancellationToken = default)
    {
        _logger.LogError($"No handler found for url: {showUrl}");
        return Task.FromResult(new Uri[0]);
    }

    public Task<Uri[]> GetShowSeasonEpisodesAsync(Uri seasonUrl, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException();
    }

    public Task<EpisodeInfo> GetEpisodeInfoAsync(Uri episodeUrl, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException();
    }
}
