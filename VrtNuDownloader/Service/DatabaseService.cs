using Microsoft.EntityFrameworkCore;
using System;
using VrtNuDownloader.Models.Sqlite;

namespace VrtNuDownloader.Service.Interface
{
    public class DatabaseService : IDatabaseService 
    {
        private readonly VrtNuDownloaderContext database;

        public DatabaseService()
        {
            database = new VrtNuDownloaderContext();
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
