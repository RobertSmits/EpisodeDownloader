using System;

namespace EpisodeDownloader.Core.Service.History
{
    public interface IHistoryService
    {
        void AddDownloaded(string episodeName, Uri episodeUrl, Uri videoUrl);
        bool CheckIfDownloaded(Uri episodeUrl);
    }
}
