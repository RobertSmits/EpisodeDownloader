using Microsoft.EntityFrameworkCore;
using VrtNuDownloader.Core.Models.Sqlite;

namespace VrtNuDownloader.Core.Service.DataBase
{
    public interface IDatabaseService
    {
        DbSet<Downloaded> GetHistory();
        void SaveChanges();
        void AddHistory(string itemName, string episodeUrl, string videoUrl);
    }
}
