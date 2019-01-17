using System;
using System.Diagnostics;
using System.IO;
using EpisodeDownloader.Core.Models;
using EpisodeDownloader.Core.Service.File;

namespace EpisodeDownloader.Core.Service.Ffmpeg
{
    public class FfmpegService : IFfmpegService
    {
        private readonly IFileService _fileService;

        public FfmpegService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public bool DownloadAndMoveEpisode(Uri streamUrl, string fileName, string downloadPath, string savePath)
        {
            return DownloadAndMoveEpisode(streamUrl, fileName, downloadPath, savePath, 0, 0);
        }

        public bool DownloadAndMoveEpisode(Uri streamUrl, string fileName, string downloadPath, string savePath, int skip, int duration)
        {
            var downloadFilePath = Path.Combine(downloadPath, fileName);
            var result = DownloadEpisode(streamUrl, downloadFilePath, skip, duration);
            if (result == false) return false;

            savePath = Path.Combine(savePath, fileName);
            _fileService.MoveFile(downloadFilePath, savePath);
            return true;
        }

        public bool DownloadEpisode(Uri streamUrl, string filePath)
        {
            return DownloadEpisode(streamUrl, filePath, 0, 0);
        }

        public bool DownloadEpisode(Uri streamUrl, string filePath, int skip, int duration)
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg";
            var argumentString = skip > 0 ? $"-ss {skip} " : "";
            argumentString += $"-i \"{streamUrl.AbsoluteUri}\" ";
            argumentString += duration > 0 ? $"-to {duration} " : "";
            argumentString += $" -c copy -copyts \"{filePath}\"";

            p.StartInfo.Arguments = argumentString;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            if (!p.Start()) return false;
            string line;
            while ((line = p.StandardError.ReadLine()) != null)
            {
                if (line.Contains("Duration: "))
                {
                    var time = line.Split(",")[0].Replace("  Duration: ", "");
                    Console.WriteLine("Lengte aflevering: " + time);
                }
            }
            p.WaitForExit();
            return p.ExitCode == 0;
        }
    }
}
