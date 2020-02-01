using System;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Contracts;
using Microsoft.Extensions.Logging;

namespace EpisodeDownloader.Core.Downloader
{
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

        public Uri[] GetShowSeasons(Uri showUrl)
        {
            _logger.LogError($"No handler found for url: {showUrl}");
            return new Uri[0];
        }

        public Uri[] GetShowSeasonEpisodes(Uri seasonUrl)
        {
            throw new InvalidOperationException();
        }

        public EpisodeInfo GetEpisodeInfo(Uri episodeUrl)
        {
            throw new InvalidOperationException();
        }
    }
}
