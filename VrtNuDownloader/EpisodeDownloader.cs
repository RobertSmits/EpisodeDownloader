using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        public EpisodeDownloader(IVrtNuService vrtNuService, ILogService logService, IHistoryService historyService, IFileService fileService)
        {
            _vrtNuService = vrtNuService;
            _logService = logService;
            _historyService = historyService;
            _fileService = fileService;
        }
        
        public void Run(IEnumerable<Uri> ShowUris)
        {
            foreach(var showUri in ShowUris)
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
                        DownloadEpisode(episode);
                    }
                }
            }
        }

        private string CreateFileName(VrtContent episodeInfo)
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

        private void DownloadEpisode(Uri episodeUri)
        {
            var episodeInfo = _vrtNuService.GetEpisodeInfo(episodeUri);
            if (_historyService.CheckIfDownloaded(episodeInfo.name))
            {
                _logService.WriteLog("Already downloaded, skipping...");
                return;
            }

            var episodeDownloadURL = _vrtNuService.GetPublishInfo(episodeInfo.publicationId, episodeInfo.videoId)
                .targetUrls.FirstOrDefault(x => x.type == "HLS")?.url;

            if (episodeDownloadURL == null)
            {
                _logService.WriteLog("Couldn't find a valid M3U8");
                return;
            }

            var filename = CreateFileName(episodeInfo);
            var filePath = Path.Combine(_fileService.DownloadDir, _fileService.MakeValidFolderName(episodeInfo.programTitle));
            _fileService.EnsureFolderExists(filePath);
            filePath = Path.Combine(filePath, _fileService.MakeValidFileName(filename));

            _logService.WriteLog($"Downloading {episodeInfo.name}");
            if (DownloadEpisode(episodeDownloadURL, filePath))
                _historyService.AddDownloaded(episodeInfo.name);
        }

        private bool DownloadEpisode(string downloadURL, string filename)
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg";
            p.StartInfo.Arguments = $"-i \"{downloadURL}\" -c copy \"{filename}\"";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            if (!p.Start())
            {
                _logService.WriteLog("Error downloading episode");
                return false;
            }
            StreamReader r = p.StandardError;
            string line;


            while ((line = r.ReadLine()) != null)
            {
                if (line.Contains("Duration: "))
                {
                    var time = line.Replace(", start: 0.000000, bitrate: N/A", "").Replace("  ", "").Replace(", start: 0.000000, bitrate: 0 kb/s", "").Replace("Duration: ", "");
                    _logService.WriteLog("Lengte aflevering: " + time);
                    //break;
                }
            }
            p.WaitForExit();
            _logService.WriteLog("Done downloading " + filename);
            return p.ExitCode == 0;


            //bool firstDuration = false;
            //while ((line = r.ReadLine()) != null)
            //{
            //    var time = string.Empty;
            //    if (line.Contains("Duration: ") && !firstDuration)
            //    {
            //        firstDuration = true;
            //        time = line.Replace(", start: 0.000000, bitrate: N/A", "").Replace("  ", "").Replace(", start: 0.000000, bitrate: 0 kb/s", "").Replace("Duration: ", "");
            //        _logService.WriteLog("Lengte aflevering: " + time);
            //    }
            //    if (line.Contains("time="))
            //    {
            //        Console.Clear();
            //        line = line.Substring(line.IndexOf("time=") + 5);
            //        string[] outputLines = line.Split(' ');
            //        line = outputLines[0];
            //        _logService.WriteLog("Downloading: " + filename);
            //        _logService.WriteLog("Progress: " + line + "/" + time);
            //    }
            //}
            p.WaitForExit();
            _logService.WriteLog("Done downloading " + filename);
            return p.ExitCode == 0;
        }
    }
}
