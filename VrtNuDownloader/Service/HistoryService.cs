using System;
using System.Linq;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class HistoryService : IHistoryService
    {
        private readonly IDatabaseService _databaseService;
        
        public HistoryService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddDownloaded(string episodeName, Uri episodeUri, Uri videoUri)
        {
            _databaseService.AddHistory(episodeName, episodeUri.AbsoluteUri, videoUri.AbsoluteUri);
        }

        public bool CheckIfDownloaded(Uri episodeUri)
        {
            return _databaseService.GetHistory().FirstOrDefault(x => x.EpisodeUrl == episodeUri.AbsoluteUri) != null;
        }

#if CHECK_EP_NAME
        public bool CheckIfDownloaded(string episodeName, Uri episodeUri, Uri videoUri)
        {
            //return _databaseService.GetHistory().FirstOrDefault(x => x.Name == episodeName) != null;
            var downoadedItem = _databaseService.GetHistory().FirstOrDefault(x => x.Name == episodeName);
            if (downoadedItem == null) return false;
            if (downoadedItem.EpisodeUrl != null) return true;
            downoadedItem.EpisodeUrl = episodeUri.AbsoluteUri;
            downoadedItem.VideoUrl = videoUri.AbsoluteUri;
            _databaseService.SaveChanges();
            return true;
        }
#endif
    }
}
