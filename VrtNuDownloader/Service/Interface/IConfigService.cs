using System.Collections.Generic;

namespace VrtNuDownloader.Service.Interface
{
    public interface IConfigService
    {
        string Email { get; }
        string Password { get; }
        string Cookie { get; set; }

        string DownloadPath { get; }
        string SavePath { get; }
        bool SaveShowsInFolders { get; }
        bool SaveSeasonsInFolders { get; }
        IEnumerable<string> WatchUrls { get; }
    }
}
