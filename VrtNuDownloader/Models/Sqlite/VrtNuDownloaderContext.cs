using Microsoft.EntityFrameworkCore;
using System;

namespace VrtNuDownloader.Models.Sqlite
{
    public class VrtNuDownloaderContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=config.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Downloaded>().ToTable("Downloaded").HasKey(d => d.Name);
        }
    }

    public class Downloaded
    {
        public string Name { get; set; }
        public string EpisodeUrl { get; set; }
        public string VideoUrl { get; set; }
        public DateTime DownloadDate { get; set; }
    }
}
