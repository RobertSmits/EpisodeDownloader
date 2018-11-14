using System;
using VrtNuDownloader.Downloader.Vrt.Models.Api;

namespace VrtNuDownloader.Downloader.Vrt.Service
{
    public interface IVrtNuService
    {
        Uri[] GetShowSeasons(Uri showUrl);
        Uri[] GetShowSeasonEpisodes(Uri seasonUrl);
        VrtContent GetEpisodeInfo(Uri episodeUrl);
        VrtPbsPub GetPublishInfo(string publicationId, string videoId);
        VrtContent GetEpisodeInfoV2(Uri episodeUrl);
        VrtPbsPubv2 GetPublishInfoV2(string publicationId, string videoId);
    }
}
