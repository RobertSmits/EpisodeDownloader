using System.Collections.Generic;

namespace VrtNuDownloader.Service.Interface
{
    public interface IConfigService
    {
        string DownloadPath { get; }
        string SavePath { get; }
        bool SaveShowsInFolders { get; }
        bool SaveSeasonsInFolders { get; }
        IEnumerable<string> WatchUrls { get; }
    }
}
