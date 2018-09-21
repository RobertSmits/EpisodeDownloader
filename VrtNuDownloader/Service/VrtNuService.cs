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
        private readonly ILogService _logService;
        private readonly IConfigService _configService;
        private VrtPlayerTokenSet _vrtPlayerTokenSet;
        private VrtTokenSet _vrtTokenSet;


        private string _playerToken
        {
            get
            {
                if (_vrtPlayerTokenSet == null || _vrtPlayerTokenSet?.expirationDate <= DateTime.Now.ToUniversalTime())
                    _vrtPlayerTokenSet = GetPlayerToken();
                return _vrtPlayerTokenSet.vrtPlayerToken;
            }
        }

        private string _vrtToken
        {
            get
            {
                if (_vrtTokenSet == null || FromUnixTime(_vrtTokenSet.expiry)<= DateTime.Now)
                {
                    _vrtTokenSet = GetTokenSet();
                    _configService.Cookie = _vrtTokenSet.refreshtoken;
                }
                return _vrtTokenSet.vrtnutoken;
            }
        }

        public VrtNuService(ILogService logService, IConfigService configService)
        {
            _logService = logService;
            _configService = configService;
        }

        private VrtPlayerTokenSet GetPlayerToken()
        {
            var url = "https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/tokens";
            var webClient = new WebClient();
            webClient.Headers.Add("cookie", $"X-VRT-Token={_vrtToken};");
            var contentJson = webClient.UploadString(url, "");
            return JsonConvert.DeserializeObject<VrtPlayerTokenSet>(contentJson);
        }

        private VrtTokenSet GetTokenSet()
        {
            var url = "https://token.vrt.be/refreshtoken";
            var webClient = new WebClient();
            webClient.Headers.Add("cookie", $"vrtlogin-rt={_configService.Cookie};");
            var contentJson = webClient.DownloadString(url);
            return JsonConvert.DeserializeObject<VrtTokenSet>(contentJson);
        }

        public List<Uri> GetShowSeasons(Uri showUri)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument html = web.Load(showUri);
            var seasonSelectOIptions = html.DocumentNode.SelectNodes("//*[@class=\"vrt-labelnav\"]")
                ?.FirstOrDefault()
                ?.SelectNodes(".//li//a");

            if ((seasonSelectOIptions?.Count ?? 0) <= 1)
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
            HtmlDocument html = new HtmlWeb().Load(seasonUri);
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

        public VrtContent GetEpisodeInfoV2(Uri episodeUri)
        {
            var epInfo = new VrtContent();
            try
            {
                epInfo = GetEpisodeInfo(episodeUri);
            }
            catch
            {
                _logService.WriteLog(MessageType.Error, "Old episode info failed");
            }
            finally
            {
                HtmlDocument html = new HtmlWeb().Load(episodeUri);
                var div = html.DocumentNode.SelectSingleNode("//div[@class='vrtvideo']");
                epInfo.publicationId = div.GetAttributeValue("data-publicationid", "");
                epInfo.videoId = div.GetAttributeValue("data-videoid", "");
            }
            return epInfo;
        }

        public VrtPbsPubv2 GetPublishInfoV2(string publicationId, string videoId)
        {
            var pbsPubURL = $"https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/videos/{publicationId}${videoId}?vrtPlayerToken={_playerToken}&client=vrtvideo";
            var pbsPubJson = new WebClient().DownloadString(pbsPubURL);
            return JsonConvert.DeserializeObject<VrtPbsPubv2>(pbsPubJson);
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
