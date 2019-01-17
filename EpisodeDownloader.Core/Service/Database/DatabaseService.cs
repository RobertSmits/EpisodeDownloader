using Microsoft.EntityFrameworkCore;
using System;
using EpisodeDownloader.Core.Models.Sqlite;

namespace EpisodeDownloader.Core.Service.DataBase
{
    public class DatabaseService : IDatabaseService
    {
        private readonly EpisodeDownloaderContext database;

        public DatabaseService()
        {
            database = new EpisodeDownloaderContext();
            database.Database.Migrate();
            var count = database.SaveChanges();
        }

        public void AddHistory(string itemName, string episodeUrl, string videoUrl)
        {
            database.Set<Downloaded>().Add(new Downloaded { Name = itemName, EpisodeUrl = episodeUrl, VideoUrl = videoUrl, DownloadDate = DateTime.Now });
            database.SaveChanges();
        }

        public DbSet<Downloaded> GetHistory()
        {
            return database.Set<Downloaded>();
        }

        public void SaveChanges()
        {
            database.SaveChanges();
        }
    }
}
