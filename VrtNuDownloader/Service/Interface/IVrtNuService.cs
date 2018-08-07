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
    }
}
