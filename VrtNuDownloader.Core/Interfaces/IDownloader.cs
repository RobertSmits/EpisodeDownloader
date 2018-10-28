using System;

namespace VrtNuDownloader.Core.Interfaces
{
    public interface IDownloader
    {
        bool CanHandleUrl(Uri episodeUrl);
        void Handle(Uri episodeUrl);
    }
}
