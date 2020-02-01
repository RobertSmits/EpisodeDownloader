using System;
using System.Collections.Generic;
using System.Linq;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Core;
using EpisodeDownloader.Core.Downloader;
using EpisodeDownloader.Core.Extensions;
using EpisodeDownloader.Core.Models;
using EpisodeDownloader.Core.Service.Ffmpeg;
using EpisodeDownloader.Core.Service.File;
using EpisodeDownloader.Core.Service.History;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EpisodeDownloader
{
    public class EpisodeDownloader
    {
        private readonly Configuration _configuration;
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly IFfmpegService _ffmpegService;
        private readonly IHistoryService _historyService;
        private readonly IEnumerable<IEpisodeProvider> _episodeProviders;
        private readonly DefaultDownloader _defaultDownloader;

        public EpisodeDownloader
            (
                IOptions<Configuration> configuration,
                ILogger<EpisodeDownloader> logger,
                IFileService fileService,
                IFfmpegService ffmpegService,
                IHistoryService historyService,
                IEnumerable<IEpisodeProvider> episodeProviders,
                DefaultDownloader defaultDownloader
            )
        {
            _configuration = configuration.Value;
            _logger = logger;
            _fileService = fileService;
            _ffmpegService = ffmpegService;
            _historyService = historyService;
            _episodeProviders = episodeProviders;
            _defaultDownloader = defaultDownloader;
        }

        public void Run()
        {
            foreach (var showUrl in _configuration.WatchUrls.Select(x => new Uri(x)))
            {
                var handler = _episodeProviders.FirstOrDefault(x => x.CanHandleUrl(showUrl))
                    ?? _defaultDownloader;

                DownloadShow(showUrl, handler);
            }
        }

        public void DownloadShow(Uri showUrl, IEpisodeProvider episodeProvider)
        {
            _logger.LogInformation("Current show: " + showUrl);
            var seasons = episodeProvider.GetShowSeasons(showUrl);
            if (seasons == null)
            {
                _logger.LogInformation("No Seasons Available");
                return;
            }

            foreach (var season in seasons)
            {
                DownloadSeason(season, episodeProvider);
            }
        }

        public void DownloadSeason(Uri seasonUrl, IEpisodeProvider episodeProvider)
        {
            _logger.LogInformation("Current season: " + seasonUrl);
            var episodes = episodeProvider.GetShowSeasonEpisodes(seasonUrl);
            if (episodes.Length == 0)
            {
                _logger.LogInformation("No Episodes Available");
                return;
            }
            foreach (var episode in episodes)
            {
                _logger.LogInformation("Current episode: " + episode);
                var downloaded = _historyService.CheckIfDownloaded(episode);
                if (downloaded)
                {
                    _logger.LogInformation("Already downloaded. Skipped");
                    continue;
                }
                try
                {
                    DownloadEpisode(episode, episodeProvider);
                    _logger.LogInformation("Downnload Finished");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        private int DownloadEpisode(Uri episodeUrl, IEpisodeProvider episodeProvider)
        {
            var episodeInfo = episodeProvider.GetEpisodeInfo(episodeUrl);
            _logger.LogInformation($"Downloading {episodeInfo.GetFileName()}");

            var filename = _fileService.MakeValidFileName(episodeInfo.GetFileName());

            var savePathBuilder = new SavePathBuilder(_fileService)
                .ForEpisode(episodeInfo)
                .SetBasePath(_configuration.SavePath);
            if (_configuration.SaveShowsInFolders)
                savePathBuilder.AddShowFolder();
            if (_configuration.SaveSeasonsInFolders)
                savePathBuilder.AddSeasonFolder();
            var savePath = savePathBuilder.Build();

            var downloadFile = new File(filename, _configuration.DownloadExtension, _configuration.DownloadPath);
            var finalFile = downloadFile.SetPath(savePath);

            _fileService.EnsureFolderExists(_configuration.DownloadPath);
            _fileService.EnsureFolderExists(savePath);

            _ffmpegService.DownloadEpisode(episodeInfo.StreamUrl, downloadFile.GetFullPath(), episodeInfo.Skip, episodeInfo.Duration);
            _fileService.MoveFile(downloadFile.GetFullPath(), finalFile.GetFullPath());

            _historyService.AddDownloaded(episodeInfo.GetFileName(), episodeUrl, episodeInfo.StreamUrl);
            return 0;
        }
    }
}
