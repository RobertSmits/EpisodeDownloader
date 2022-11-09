using System;

namespace EpisodeDownloader.Core.Service.Download;

public interface IDownloadService
{
    void DownloadEpisode(Uri streamUrl, string filePath);
    void DownloadEpisode(Uri streamUrl, string filePath, TimeSpan skip, TimeSpan duration);
}
