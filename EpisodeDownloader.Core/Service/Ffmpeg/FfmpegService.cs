using System;
using System.Diagnostics;
using EpisodeDownloader.Contracts.Exceptions;
using Microsoft.Extensions.Logging;

namespace EpisodeDownloader.Core.Service.Ffmpeg
{
    public class FfmpegService : IFfmpegService
    {
        private readonly ILogger _logger;

        public FfmpegService(ILogger<FfmpegService> logger)
        {
            _logger = logger;
        }

        public void DownloadEpisode(Uri streamUrl, string filePath)
        {
            DownloadEpisode(streamUrl, filePath, TimeSpan.Zero, TimeSpan.Zero);
        }

        public void DownloadEpisode(Uri streamUrl, string filePath, TimeSpan skip, TimeSpan duration)
        {
            var p = new Process();
            p.StartInfo.FileName = "ffmpeg";
            var argumentString = "-y -hide_banner -loglevel panic ";
            argumentString += $"-i \"{streamUrl.AbsoluteUri}\" ";
            argumentString += skip.TotalSeconds > 0 ? $"-ss {skip.TotalSeconds} " : "";
            argumentString += duration.TotalSeconds > 0 ? $"-to {duration.TotalSeconds} " : "";
            argumentString += $" -c copy -copyts \"{filePath}\"";

            _logger.LogDebug($"Running ffmpeg: ffmpeg {argumentString}");

            p.StartInfo.Arguments = argumentString;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            if (!p.Start())
                throw new DownloadException("Error running ffmpeg");

            p.WaitForExit();

            if (p.ExitCode != 0)
                throw new DownloadException("Error running ffmpeg");
        }
    }
}
