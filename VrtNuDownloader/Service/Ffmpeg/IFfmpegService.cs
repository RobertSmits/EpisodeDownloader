using System;

namespace VrtNuDownloader.Service.Interface
{
    public interface IFfmpegService
    {
        bool DownloadEpisode(Uri videoUri, string fileName);
    }
}
