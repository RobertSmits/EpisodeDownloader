using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using VrtNuDownloader.Models;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class VrtNuService : IVrtNuService
    {
        public List<Uri> GetShowSeasons(Uri showUri)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument html = web.Load(showUri);
            var seasonSelectOIptions = html.DocumentNode.SelectNodes("//*[@class=\"vrt-labelnav\"]")
                ?.FirstOrDefault()
                ?.SelectNodes(".//li//a");

            if ((seasonSelectOIptions?.Count ?? 0)  <= 1)
            {
                return new List<Uri> { showUri };
                //return new List<Uri> {
                //    new Uri(showUri.AbsoluteUri.Replace(".relevant/", "") + seasonSelectOIptions[0].InnerHtml.Replace("Seizoen ", "/") + ".lists.all-episodes/")
                //};
            }

            return seasonSelectOIptions
                .Select(x => new Uri("https://www.vrt.be" + x.GetAttributeValue("href", "")))
                .OrderBy(x => x.AbsoluteUri)
                .ToList();
        }
        public List<Uri> GetShowSeasonEpisodes(Uri seasonUri)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument html = web.Load(seasonUri);
            return html.DocumentNode.SelectSingleNode("//ul[@aria-labelledby='episodelist-title']")
                ?.SelectNodes(".//li//a")
                ?.Select(x => new Uri("https://www.vrt.be" + x.GetAttributeValue("href", "")))
                .OrderBy(x => x.AbsoluteUri)
                .ToList();
        }

        public VrtContent GetEpisodeInfo(Uri episodeUri)
        {
            var episodeURL = episodeUri.AbsoluteUri;
            var contentJsonURL = episodeURL.Remove(episodeURL.Length - 1) + ".content.json";
            var contentJson = new WebClient().DownloadString(contentJsonURL);
            return JsonConvert.DeserializeObject<VrtContent>(contentJson);
        }

        public VrtPbsPub GetPublishInfo(string publicationId, string videoId)
        {
            var pbsPubURL = $"https://mediazone.vrt.be/api/v1/vrtvideo/assets/{publicationId}${videoId}";
            var pbsPubJson = new WebClient().DownloadString(pbsPubURL);
            return JsonConvert.DeserializeObject<VrtPbsPub>(pbsPubJson);
        }
    }
}
