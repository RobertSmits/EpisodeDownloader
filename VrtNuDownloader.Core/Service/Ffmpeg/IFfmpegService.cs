using System;

namespace VrtNuDownloader.Core.Service.Ffmpeg
{
    public interface IFfmpegService
    {
        bool DownloadEpisode(Uri videoUri, string fileName);
    }
}
