using System;

namespace VrtNuDownloader.Core.Service.Ffmpeg
{
    public interface IFfmpegService
    {
        bool DownloadEpisode(Uri videoUri, string filePath);
        bool DownloadAndMoveEpisode(Uri videoUri, string fileName, string downloadPath, string savePath);
    }
}
