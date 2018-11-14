using System;

namespace VrtNuDownloader.Core.Service.Ffmpeg
{
    public interface IFfmpegService
    {
        bool DownloadEpisode(Uri streamUrl, string filePath);
        bool DownloadAndMoveEpisode(Uri streamUrl, string fileName, string downloadPath, string savePath);
    }
}
