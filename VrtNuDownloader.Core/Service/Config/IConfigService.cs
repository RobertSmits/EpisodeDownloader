using System.Collections.Generic;

namespace VrtNuDownloader.Core.Service.Config
{
    public interface IConfigService
    {
        // Vrt
        string VrtCookie { get; set; }

        // Vier
        string VierLastAuthUser { get; }
        string VierRefreshToken { get; }

        // Application
        string DownloadPath { get; }
        string SavePath { get; }
        bool SaveShowsInFolders { get; }
        bool SaveSeasonsInFolders { get; }
        IEnumerable<string> WatchUrls { get; }
    }
}
