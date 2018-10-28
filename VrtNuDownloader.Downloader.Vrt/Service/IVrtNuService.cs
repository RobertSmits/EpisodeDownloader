using System;
using VrtNuDownloader.Downloader.Vrt.Models.Api;

namespace VrtNuDownloader.Downloader.Vrt.Service
{
    public interface IVrtNuService
    {
        Uri[] GetShowSeasons(Uri showUri);
        Uri[] GetShowSeasonEpisodes(Uri seasonUri);
        VrtContent GetEpisodeInfo(Uri episodeUri);
        VrtPbsPub GetPublishInfo(string publicationId, string videoId);
        VrtContent GetEpisodeInfoV2(Uri episodeUri);
        VrtPbsPubv2 GetPublishInfoV2(string publicationId, string videoId);
    }
}
