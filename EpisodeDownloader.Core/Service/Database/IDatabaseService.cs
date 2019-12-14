﻿using EpisodeDownloader.Core.Models.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EpisodeDownloader.Core.Service.DataBase
{
    public interface IDatabaseService
    {
        DbSet<Downloaded> GetHistory();
        void SaveChanges();
        void AddHistory(string itemName, string episodeUrl, string videoUrl);
    }
}
