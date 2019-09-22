using System;
using EpisodeDownloader.Downloader.Vrt.Models.Api;

namespace EpisodeDownloader.Downloader.Vrt.Service
{
    public interface IVrtNuService
    {
        Uri[] GetShowSeasons(Uri showUrl);
        Uri[] GetShowSeasonEpisodes(Uri seasonUrl);
        VrtContent GetEpisodeInfo(Uri episodeUrl);
        VrtPbsPubV2 GetPublishInfo(string publicationId, string videoId);
    }
}
