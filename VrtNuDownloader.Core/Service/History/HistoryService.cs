using System;
using System.Linq;
using VrtNuDownloader.Core.Service.DataBase;

namespace VrtNuDownloader.Core.Service.History
{
    public class HistoryService : IHistoryService
    {
        private readonly IDatabaseService _databaseService;

        public HistoryService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddDownloaded(string episodeName, Uri episodeUrl, Uri videoUrl)
        {
            _databaseService.AddHistory(episodeName, episodeUrl.AbsoluteUri, videoUrl.AbsoluteUri);
        }

        public bool CheckIfDownloaded(Uri episodeUrl)
        {
            return _databaseService.GetHistory().FirstOrDefault(x => x.EpisodeUrl == episodeUrl.AbsoluteUri) != null;
        }

#if CHECK_EP_NAME
        public bool CheckIfDownloaded(string episodeName, Uri episodeUrl, Uri videoUrl)
        {
            //return _databaseService.GetHistory().FirstOrDefault(x => x.Name == episodeName) != null;
            var downoadedItem = _databaseService.GetHistory().FirstOrDefault(x => x.Name == episodeName);
            if (downoadedItem == null) return false;
            if (downoadedItem.EpisodeUrl != null) return true;
            downoadedItem.EpisodeUrl = episodeUrl.AbsoluteUri;
            downoadedItem.VideoUrl = videoUrl.AbsoluteUri;
            _databaseService.SaveChanges();
            return true;
        }
#endif
    }
}
