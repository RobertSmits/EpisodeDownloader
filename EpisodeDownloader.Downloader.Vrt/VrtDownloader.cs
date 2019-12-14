using System;
using System.Linq;
using EpisodeDownloader.Core.Downloader;
using EpisodeDownloader.Core.Extensions;
using EpisodeDownloader.Core.Models;
using EpisodeDownloader.Core.Service.History;
using EpisodeDownloader.Downloader.Vrt.Models.Api;
using EpisodeDownloader.Downloader.Vrt.Service;
using Microsoft.Extensions.Logging;

namespace EpisodeDownloader.Downloader.Vrt
{
    public class VrtDownloader : IDownloader
    {
        private readonly ILogger _logger;
        private readonly IHistoryService _historyService;
        private readonly IVrtNuService _vrtNuService;

        public VrtDownloader
            (
                ILogger<VrtDownloader> logger,
                IHistoryService historyService,
                IVrtNuService vrtNuService
            )
        {
            _logger = logger;
            _historyService = historyService;
            _vrtNuService = vrtNuService;
        }

        public bool CanHandleUrl(Uri episodeUrl)
        {
            return episodeUrl.AbsoluteUri.Contains("vrt.be");
        }

        public void Handle(Uri episodeUrl)
        {
            _logger.LogInformation("Current show: " + episodeUrl);
            var seasons = _vrtNuService.GetShowSeasons(episodeUrl);
            if (seasons == null)
            {
                _logger.LogInformation("No Seasons Available");
                return;
            }

            foreach (var season in seasons)
            {
                var episodes = _vrtNuService.GetShowSeasonEpisodes(season);
                if (episodes == null)
                {
                    _logger.LogInformation("No Episodes Available");
                    continue;
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
        }

        private int DownloadEpisode(Uri episodeUrl)
        {
            var episodeInfo = _vrtNuService.GetEpisodeInfo(episodeUrl);
            var pubInfo = default(VrtPbsPubV2);
            try
            {
                pubInfo = _vrtNuService.GetPublishInfo(episodeInfo.publicationId, episodeInfo.videoId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while downloading {episodeInfo.name}. StackTrace: {e.ToString()} {e.StackTrace}");
                return 3;
            }
            var episodeDownloadUrl = pubInfo.targetUrls.Where(x => x.type.ToLower() == "hls")
                                        .Select(x => new Uri(x.url)).FirstOrDefault();
            if (episodeDownloadUrl == null) return 1;

#if CHECK_EP_NAME
            if (_historyService.CheckIfDownloaded(episodeInfo.name, episodeUrl, episodeDownloadUrl)) return -1;
#endif

            var epInfo = new EpisodeInfo
            {
                ShowName = episodeInfo.programTitle,
                Season = episodeInfo.seasonTitle,
                Episode = episodeInfo.episodeNumber,
                Title = episodeInfo.title,
                StreamUrl = episodeDownloadUrl
            };

            _logger.LogInformation($"Downloading {episodeInfo.name}");

            var skip = pubInfo.playlist.content.TakeWhile(x => !x.skippable).Sum(x => x.duration) / 1000;
            var duration = (pubInfo.playlist.content.FirstOrDefault(x => x.skippable)?.duration ?? 0) / 1000;
            var processOutput = epInfo.DownloadToFolder(skip, duration);
            if (!processOutput) return 2;

            _historyService.AddDownloaded(episodeInfo.name, episodeUrl, episodeDownloadUrl);
            return 0;
        }
    }
}
