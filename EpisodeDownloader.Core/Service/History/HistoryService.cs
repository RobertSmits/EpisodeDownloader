using System;
using System.Linq;
using EpisodeDownloader.Core.Models.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EpisodeDownloader.Core.Service.History
{
    public class HistoryService : IHistoryService
    {
        private readonly EpisodeDownloaderContext database;

        public HistoryService()
        {
            database = new EpisodeDownloaderContext();
            database.Database.Migrate();
            database.SaveChanges();
        }

        public void AddDownloaded(string episodeName, Uri episodeUrl, Uri videoUrl)
        {
            database.Set<Downloaded>().Add(new Downloaded { Name = episodeName, EpisodeUrl = episodeUrl.AbsoluteUri, VideoUrl = videoUrl.AbsoluteUri, DownloadDate = DateTime.Now });
            database.SaveChanges();
        }

        public bool CheckIfDownloaded(Uri episodeUrl)
        {
            return database.Set<Downloaded>().FirstOrDefault(x => x.EpisodeUrl == episodeUrl.AbsoluteUri) != null;
        }
    }
}
