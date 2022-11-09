using System;
using System.Threading.Tasks;
using EpisodeDownloader.Core.Models.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EpisodeDownloader.Core.Service.History;

public class HistoryService : IHistoryService
{
    private readonly EpisodeDownloaderContext database;

    public HistoryService()
    {
        database = new EpisodeDownloaderContext();
        database.Database.Migrate();
        database.SaveChanges();
    }

    public async Task AddDownloadedAsync(string episodeName, Uri episodeUrl, Uri videoUrl)
    {
        database.Set<Downloaded>().Add(new Downloaded { Name = episodeName, EpisodeUrl = episodeUrl.AbsoluteUri, VideoUrl = videoUrl.AbsoluteUri, DownloadDate = DateTime.Now });
        await database.SaveChangesAsync();
    }

    public async Task<bool> CheckIfDownloadedAsync(Uri episodeUrl)
    {
        return await database.Set<Downloaded>().FirstOrDefaultAsync(x => x.EpisodeUrl == episodeUrl.AbsoluteUri) != null;
    }
}
