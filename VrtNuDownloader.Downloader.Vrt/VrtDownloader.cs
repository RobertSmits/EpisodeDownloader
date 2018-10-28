using System;
using System.IO;
using System.Linq;
using VrtNuDownloader.Core.Interfaces;
using VrtNuDownloader.Core.Service.Config;
using VrtNuDownloader.Core.Service.Ffmpeg;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Core.Service.History;
using VrtNuDownloader.Core.Service.Logging;
using VrtNuDownloader.Downloader.Vrt.Models.Api;
using VrtNuDownloader.Downloader.Vrt.Service;

namespace VrtNuDownloader.Downloader.Vrt
{
    public class VrtDownloader : IDownloader
    {
        private readonly ILoggingService _logService;
        private readonly IFileService _fileService;
        private readonly IVrtNuService _vrtNuService;
        private readonly IFfmpegService _ffmpegService;
        private readonly IConfigService _configService;
        private readonly IHistoryService _historyService;

        public VrtDownloader
            (
                ILoggingService logService,
                IFileService fileService,
                IVrtNuService vrtNuService,
                IFfmpegService ffmpegService,
                IConfigService configService,
                IHistoryService historyService
            )
        {
            _logService = logService;
            _fileService = fileService;
            _vrtNuService = vrtNuService;
            _ffmpegService = ffmpegService;
            _configService = configService;
            _historyService = historyService;
        }

        public bool CanHandleUrl(Uri episodeUrl)
        {
            return episodeUrl.AbsoluteUri.Contains("vrt.be");
        }

        public void Handle(Uri episodeUrl)
        {
            _logService.WriteLog(MessageType.Info, "Current show: " + episodeUrl);
            var seasons = _vrtNuService.GetShowSeasons(episodeUrl);
            if (seasons == null)
            {
                _logService.WriteLog(MessageType.Warn, "No Seasons Available");
                return;
            }

            foreach (var season in seasons)
            {
                var episodes = _vrtNuService.GetShowSeasonEpisodes(season);
                if (episodes == null)
                {
                    _logService.WriteLog(MessageType.Warn, "No Episodes Available");
                    continue;
                }
                foreach (var episode in episodes)
                {
                    var status = _historyService.CheckIfDownloaded(episode) ? -1 : DownloadEpisode(episode);
                    // if (status == -1) _logService.WriteLog("Already downloaded, skipped");
                    if (status == 0) _logService.WriteLog(MessageType.Info, "Downnload Finished");
                    if (status == 1) _logService.WriteLog(MessageType.Error, "Couldn't find a valid M3U8");
                    if (status == 2) _logService.WriteLog(MessageType.Error, "Error running ffmpeg");
                }
            }
        }

        private string GetFileName(VrtContent episodeInfo)
        {
            var filename = episodeInfo.programTitle;
            if (int.TryParse(episodeInfo.seasonTitle, out int seasonNr))
            {
                var episodeNumber = episodeInfo.episodeNumber < 100 ? episodeInfo.episodeNumber.ToString("00") : episodeInfo.episodeNumber.ToString("000");
                filename += ($" - S{seasonNr:00}E{episodeNumber} - {episodeInfo.title}.mp4");
            }
            else
            {
                filename += ($" - S{episodeInfo.seasonTitle:00}E{episodeInfo.episodeNumber} - {episodeInfo.title}.mp4");
            }
            return filename;
        }

        private string GetSavePath(VrtContent episodeInfo)
        {
            if (!_configService.SaveShowsInFolders) return _configService.SavePath;
            var path = Path.Combine(_configService.SavePath, _fileService.MakeValidFileName(episodeInfo.programTitle));
            if (!_configService.SaveSeasonsInFolders) return path;
            var season = int.TryParse(episodeInfo.seasonTitle, out int seasonNr) ? $"S{seasonNr:00}" : episodeInfo.seasonTitle;
            path = Path.Combine(path, _fileService.MakeValidFileName(season));
            return path;
        }

        private int DownloadEpisode(Uri episodeUri)
        {
            var episodeInfo = _vrtNuService.GetEpisodeInfoV2(episodeUri);
            var pubInfo = default(VrtPbsPubv2);
            try
            {
                pubInfo = _vrtNuService.GetPublishInfoV2(episodeInfo.publicationId, episodeInfo.videoId);
            }
            catch (Exception e)
            {
                _logService.WriteLog(MessageType.Error, $"Error while downloading {episodeInfo.name}. StackTrace: {e.ToString()} {e.StackTrace}");
                return 3;
            }
            var episodeDownloadUri = pubInfo.targetUrls.Where(x => x.type.ToLower() == "hls")
                                        .Select(x => new Uri(x.url)).FirstOrDefault();
            if (episodeDownloadUri == null) return 1;

#if CHECK_EP_NAME
            if (_historyService.CheckIfDownloaded(episodeInfo.name, episodeUri, episodeDownloadUri)) return -1;
#endif

            var filename = _fileService.MakeValidFileName(GetFileName(episodeInfo));
            _fileService.EnsureFolderExists(_configService.DownloadPath);
            var downloadFile = Path.Combine(_configService.DownloadPath, filename);

            _logService.WriteLog(MessageType.Info, $"Downloading {episodeInfo.name}");
            var processOutput = _ffmpegService.DownloadEpisode(episodeDownloadUri, downloadFile);
            if (!processOutput) return 2;

            var savePath = GetSavePath(episodeInfo);
            _fileService.EnsureFolderExists(savePath);
            savePath = Path.Combine(savePath, filename);
            _fileService.MoveFile(downloadFile, savePath);

            _historyService.AddDownloaded(episodeInfo.name, episodeUri, episodeDownloadUri);
            return 0;
        }
    }
}
