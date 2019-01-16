using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using VrtNuDownloader.Core.Downloader;
using VrtNuDownloader.Core.Extensions;
using VrtNuDownloader.Core.Service.History;
using VrtNuDownloader.Downloader.Vier.Service;

namespace VrtNuDownloader.Downloader.Vier
{
    public class VierDownloader : IDownloader
    {
        private readonly ILogger _logger;
        private readonly IHistoryService _historyService;
        private readonly IVierService _vierService;

        public VierDownloader
            (
                ILogger<VierDownloader> logger,
                IHistoryService historyService,
                IVierService vierService
            )
        {
            _logger = logger;
            _historyService = historyService;
            _vierService = vierService;
        }

        public bool CanHandleUrl(Uri episodeUrl)
        {
            return episodeUrl.AbsoluteUri.Contains("vier.be")
                || episodeUrl.AbsoluteUri.Contains("vijf.be")
                || episodeUrl.AbsoluteUri.Contains("zestv.be");
        }

        public void Handle(Uri episodeUrl)
        {
            _logger.LogInformation("Current show: " + episodeUrl);
            var episodes = _vierService.GetShowSeasonEpisodes(episodeUrl);
            if (!episodes.Any())
            {
                _logger.LogInformation("No Episodes Available");
                return;
            }
            foreach (var episode in episodes)
            {
                _logger.LogInformation("Current url: " + episode.ToString());
                var status = _historyService.CheckIfDownloaded(episode) ? -1 : DownloadEpisode(episode);
                if (status == -1) _logger.LogInformation("Already downloaded(skipped");
                if (status == 0) _logger.LogInformation("Downnload Finished");
                if (status == 1) _logger.LogError("Couldn't find a valid M3U8");
                if (status == 2) _logger.LogError("Error running ffmpeg");
            }
        }

        private int DownloadEpisode(Uri episodeUrl)
        {
            var epInfo = _vierService.GetEpisodeInfo(episodeUrl);
            _logger.LogInformation($"Downloading {epInfo.GetFileName()}");
            var processOutput = epInfo.DownloadToFolder();
            if (!processOutput) return 2;

            _historyService.AddDownloaded(episodeUrl.AbsolutePath.Split('/').Last(), episodeUrl, epInfo.StreamUrl);
            return 0;
        }
    }
}
