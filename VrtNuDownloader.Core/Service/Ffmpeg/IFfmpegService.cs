using System;
using VrtNuDownloader.Core.Models;

namespace VrtNuDownloader.Core.Service.Ffmpeg
{
    public interface IFfmpegService
    {
        bool DownloadEpisode(Uri streamUrl, string filePath);
        bool DownloadEpisode(Uri streamUrl, string filePath, int skip, int duration);
        bool DownloadAndMoveEpisode(Uri streamUrl, string fileName, string downloadPath, string savePath);
        bool DownloadAndMoveEpisode(Uri streamUrl, string fileName, string downloadPath, string savePath, int skip, int duration);
    }
}
