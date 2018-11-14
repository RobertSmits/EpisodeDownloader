using System;

namespace VrtNuDownloader.Core.Service.History
{
    public interface IHistoryService
    {
        void AddDownloaded(string episodeName, Uri episodeUrl, Uri videoUrl);
        bool CheckIfDownloaded(Uri episodeUrl);

#if CHECK_EP_NAME
        bool CheckIfDownloaded(string episodeName, Uri episodeUrl, Uri videoUrl);
#endif
    }
}
