using Microsoft.EntityFrameworkCore;
using System;
using VrtNuDownloader.Models.Sqlite;

namespace VrtNuDownloader.Service.Interface
{
    public interface IDatabaseService
    {
        DbSet<Downloaded> GetHistory();
        void SaveChanges();
        void AddHistory(string itemName, string episodeUrl, string videoUrl);
    }
}
