using System;
using VrtNuDownloader.Core.Models;

namespace VrtNuDownloader.Downloader.Vier.Service
{
    public interface IVierService
    {
        Uri[] GetShowSeasonEpisodes(Uri seasonUrl);
        EpisodeInfo GetEpisodeInfo(Uri episodeUrl);
        string GetEpisodeId(Uri episodeUrl);
        Uri GetStreamUrl(string episodeId);
    }
}
