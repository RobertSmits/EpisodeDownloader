using System;

namespace EpisodeDownloader.Core.Service.Ffmpeg
{
    public interface IFfmpegService
    {
        void DownloadEpisode(Uri streamUrl, string filePath);
        void DownloadEpisode(Uri streamUrl, string filePath, TimeSpan skip, TimeSpan duration);
    }
}
