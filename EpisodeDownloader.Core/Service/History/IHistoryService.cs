using System;
using System.Threading.Tasks;

namespace EpisodeDownloader.Core.Service.History
{
    public interface IHistoryService
    {
        Task AddDownloadedAsync(string episodeName, Uri episodeUrl, Uri videoUrl);
        Task<bool> CheckIfDownloadedAsync(Uri episodeUrl);
    }
}
