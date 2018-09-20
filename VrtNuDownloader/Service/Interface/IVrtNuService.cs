using System;
using System.Collections.Generic;
using VrtNuDownloader.Models;

namespace VrtNuDownloader.Service.Interface
{
    public interface IVrtNuService
    {
        List<Uri> GetShowSeasons(Uri showUri);
        List<Uri> GetShowSeasonEpisodes(Uri seasonUri);
        VrtContent GetEpisodeInfo(Uri episodeUri);
        VrtPbsPub GetPublishInfo(string publicationId, string videoId);
        VrtContent GetEpisodeInfoV2(Uri episodeUri);
        VrtPbsPubv2 GetPublishInfoV2(string publicationId, string videoId);
    }
}
