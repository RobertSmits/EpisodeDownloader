using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task Run(CancellationToken cancellationToken = default)
        {
            foreach (var showUrl in _configuration.WatchUrls.Select(x => new Uri(x)))
            {
                var handler = _episodeProviders.FirstOrDefault(x => x.CanHandleUrl(showUrl))
                    ?? _defaultDownloader;

                await DownloadShow(showUrl, handler, cancellationToken);
            }
        }

        public async Task DownloadShow(Uri showUrl, IEpisodeProvider episodeProvider, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Current show: " + showUrl);
            var seasons = await episodeProvider.GetShowSeasonsAsync(showUrl, cancellationToken);
            if (seasons == null)
            {
                _logger.LogInformation("No Seasons Available");
                return;
            }

            foreach (var season in seasons)
            {
                await DownloadSeason(season, episodeProvider);
            }
        }

        public async Task DownloadSeason(Uri seasonUrl, IEpisodeProvider episodeProvider, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Current season: " + seasonUrl);
            var episodes = await episodeProvider.GetShowSeasonEpisodesAsync(seasonUrl, cancellationToken);
            if (episodes.Length == 0)
            {
                _logger.LogInformation("No Episodes Available");
                return;
            }
            foreach (var episode in episodes)
            {
                _logger.LogInformation("Current episode: " + episode);
                var downloaded = await _historyService.CheckIfDownloadedAsync(episode);
                if (downloaded)
                {
                    _logger.LogInformation("Already downloaded. Skipped");
                    continue;
                }
                try
                {
                    await DownloadEpisode(episode, episodeProvider, cancellationToken);
                    _logger.LogInformation("Downnload Finished");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        private async Task DownloadEpisode(Uri episodeUrl, IEpisodeProvider episodeProvider, CancellationToken cancellationToken = default)
        {
            var episodeInfo = await episodeProvider.GetEpisodeInfoAsync(episodeUrl, cancellationToken);
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
            _fileService.MoveFile(downloadFile.GetFullPath(), finalFile.GetFullPath(), _configuration.Overwrite);

            await _historyService.AddDownloadedAsync(episodeInfo.GetFileName(), episodeUrl, episodeInfo.StreamUrl);
        }
    }
}
