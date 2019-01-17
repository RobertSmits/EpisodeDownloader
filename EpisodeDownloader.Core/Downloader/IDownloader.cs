using System;

namespace EpisodeDownloader.Core.Downloader
{
    public interface IDownloader
    {
        bool CanHandleUrl(Uri episodeUrl);
        void Handle(Uri episodeUrl);
    }
}
