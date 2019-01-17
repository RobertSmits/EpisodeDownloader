using System;
using Microsoft.Extensions.Logging;

namespace EpisodeDownloader.Core.Downloader
{
    public class DefaultDownloader : IDownloader
    {
        private readonly ILogger _logger;

        public DefaultDownloader(ILogger<DefaultDownloader> logger)
        {
            _logger = logger;
        }

        public bool CanHandleUrl(Uri episodeUrl)
        {
            return false;
        }

        public void Handle(Uri episodeUrl)
        {
            _logger.LogError($"No handler found for url: {episodeUrl}");
        }
    }
}
