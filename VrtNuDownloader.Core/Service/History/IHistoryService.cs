using System;

namespace VrtNuDownloader.Core.Service.History
{
    public interface IHistoryService
    {
        void AddDownloaded(string episodeName, Uri episodeUri, Uri videoUri);
        bool CheckIfDownloaded(Uri episodeUri);

#if CHECK_EP_NAME
        bool CheckIfDownloaded(string episodeName, Uri episodeUri, Uri videoUri);
#endif
    }
}
