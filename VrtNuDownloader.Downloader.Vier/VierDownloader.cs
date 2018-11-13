using System;
using System.Linq;
using VrtNuDownloader.Core.Interfaces;
using VrtNuDownloader.Core.Service.Config;
using VrtNuDownloader.Core.Service.Ffmpeg;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Core.Service.History;
using VrtNuDownloader.Core.Service.Logging;
using VrtNuDownloader.Downloader.Vier.Service;

namespace VrtNuDownloader.Downloader.Vier
{
    public class VierDownloader : IDownloader
    {
        private readonly ILoggingService _logService;
        private readonly IFileService _fileService;
        private readonly IFfmpegService _ffmpegService;
        private readonly IConfigService _configService;
        private readonly IHistoryService _historyService;
        private readonly IVierService _vierService;

        public VierDownloader
            (
                ILoggingService logService,
                IFileService fileService,
                IFfmpegService ffmpegService,
                IConfigService configService,
                IHistoryService historyService,
                IVierService vierService
            )
        {
            _logService = logService;
            _fileService = fileService;
            _ffmpegService = ffmpegService;
            _configService = configService;
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
            _logService.WriteLog(MessageType.Info, "Current show: " + episodeUrl);
            var episodes = _vierService.GetShowSeasonEpisodes(episodeUrl);
            if (episodes == null)
            {
                _logService.WriteLog(MessageType.Info, "No Episodes Available");
                return;
            }
            foreach (var episode in episodes)
            {
                _logService.WriteLog(MessageType.Info, "Current url: " + episode.ToString());
                var status = _historyService.CheckIfDownloaded(episode) ? -1 : DownloadEpisode(episode);
                if (status == -1) _logService.WriteLog(MessageType.Info, "Already downloaded, skipped");
                if (status == 0) _logService.WriteLog(MessageType.Info, "Downnload Finished");
                if (status == 1) _logService.WriteLog(MessageType.Error, "Couldn't find a valid M3U8");
                if (status == 2) _logService.WriteLog(MessageType.Error, "Error running ffmpeg");
            }
        }

        private int DownloadEpisode(Uri episodeUrl)
        {
            var epInfo = _vierService.GetEpisodeInfo(episodeUrl);
            _logService.WriteLog(MessageType.Info, $"Downloading {epInfo.GetFileName()}");
            var processOutput = epInfo.DownloadToFolder(_fileService, _configService, _ffmpegService);
            if (!processOutput) return 2;

            _historyService.AddDownloaded(episodeUrl.AbsolutePath.Split('/').Last(), episodeUrl, epInfo.StreamUrl);
            return 0;
        }
    }
}
