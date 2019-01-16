using System;

namespace VrtNuDownloader.Core.Downloader
{
    public interface IDownloader
    {
        bool CanHandleUrl(Uri episodeUrl);
        void Handle(Uri episodeUrl);
    }
}
