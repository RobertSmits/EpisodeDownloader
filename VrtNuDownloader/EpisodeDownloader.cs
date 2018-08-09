using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VrtNuDownloader.Models;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader
{
    public class EpisodeDownloader
    {
        private readonly IVrtNuService _vrtNuService;
        private readonly ILogService _logService;
        private readonly IHistoryService _historyService;
        private readonly IFileService _fileService;
        private readonly IFfmpegService _ffmpegService;

        public EpisodeDownloader
            (
                IVrtNuService vrtNuService,
                ILogService logService,
                IHistoryService historyService,
                IFileService fileService,
                IFfmpegService ffmpegService
            )
        {
            _vrtNuService = vrtNuService;
            _logService = logService;
            _historyService = historyService;
            _fileService = fileService;
            _ffmpegService = ffmpegService;
        }

        public void Run(IEnumerable<Uri> ShowUris)
        {
            foreach (var showUri in ShowUris)
            {
                _logService.WriteLog("Current show: " + showUri);
                var seasons = _vrtNuService.GetShowSeasons(showUri);
                if (seasons == null)
                {
                    _logService.WriteLog("No Seasons Available");
                    continue;
                }

                foreach (var season in seasons)
                {
                    var episodes = _vrtNuService.GetShowSeasonEpisodes(season);
                    if (episodes == null)
                    {
                        _logService.WriteLog("No Episodes Available");
                        continue;
                    }
                    foreach (var episode in episodes)
                    {
                        var status = DownloadEpisode(episode);
                        if (status == -1) _logService.WriteLog("Already downloaded, skipped");
                        if (status == 1) _logService.WriteLog("Couldn't find a valid M3U8");
                        if (status == 2) _logService.WriteLog("Error running ffmpeg");
                        if (status == 0) _logService.WriteLog("Downnload Finished");                            
                    }
                }
            }
        }

        private string GetFileName(VrtContent episodeInfo)
        {
            var filename = episodeInfo.programTitle;
            if (Int32.TryParse(episodeInfo.seasonTitle, out int seasonNr))
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

        private int DownloadEpisode(Uri episodeUri)
        {
            var episodeInfo = _vrtNuService.GetEpisodeInfo(episodeUri);
            if (_historyService.CheckIfDownloaded(episodeInfo.name)) return -1;

            var episodeDownloadUri = _vrtNuService.GetPublishInfo(episodeInfo.publicationId, episodeInfo.videoId)
                .targetUrls.Where(x => x.type == "HLS")
                .Select(x => new Uri(x.url)).FirstOrDefault();

            if (episodeDownloadUri == null) return 1;

            var filename = GetFileName(episodeInfo);
            var filePath = Path.Combine(_fileService.DownloadDir, _fileService.MakeValidFolderName(episodeInfo.programTitle));
            _fileService.EnsureFolderExists(filePath);
            filePath = Path.Combine(filePath, _fileService.MakeValidFileName(filename));

            _logService.WriteLog($"Downloading {episodeInfo.name}");
            var processOutput = _ffmpegService.DownloadEpisode(episodeDownloadUri, filePath);
            if (processOutput) _historyService.AddDownloaded(episodeInfo.name);
            return processOutput ? 0 : 2;
        }
    }
}
