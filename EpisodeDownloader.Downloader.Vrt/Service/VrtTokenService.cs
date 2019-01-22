using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using EpisodeDownloader.Core.Models;
using EpisodeDownloader.Core.Service.File;
using EpisodeDownloader.Downloader.Vrt.Models.Auth;
using EpisodeDownloader.Downloader.Vrt.Extensions;

namespace EpisodeDownloader.Downloader.Vrt.Service
{
    public class VrtTokenService : IVrtTokenService
    {
        const string GIGYA_API_KEY = "3_0Z2HujMtiWq_pkAjgnS2Md2E11a1AwZjYiBETtwNE-EoEHDINgtnvcAOpNgmrVGy";

        private VrtPlayerTokenSet _vrtPlayerTokenSet;
        private readonly ILogger _logger;
        private readonly VrtConfiguration _configuration;

        public VrtTokenService
            (
                ILogger<VrtTokenService> logger,
                IOptionsMonitor<VrtConfiguration> configMonitor
            )
        {
            _logger = logger;
            _configuration = configMonitor.CurrentValue;
        }

        public string VrtToken
        {
            get
            {
                return GetVrtToken(_configuration.Email, _configuration.Password);
            }
        }

        public string PlayerToken
        {
            get
            {
                if (_vrtPlayerTokenSet == null || _vrtPlayerTokenSet?.expirationDate <= DateTime.Now.ToUniversalTime())
                    _vrtPlayerTokenSet = RefreshPlayerToken();
                return _vrtPlayerTokenSet.vrtPlayerToken;
            }
        }

        private GigyaAuthResponse GetGigyaAuth(string username, string password)
        {
            _logger.LogDebug("Logging in to Gigya");
            _logger.LogTrace("Username: " + username);
            _logger.LogTrace("Password: " + password);
            var url = new Uri("https://accounts.eu1.gigya.com/accounts.login")
                .AddParameter("APIKey", GIGYA_API_KEY)
                .AddParameter("targetEnv", "jssdk")
                .AddParameter("loginID", username)
                .AddParameter("password", password)
                .AddParameter("authMode", "cookie");
            var webClient = new WebClient();
            var contentJson = webClient.DownloadString(url);
            return JsonConvert.DeserializeObject<GigyaAuthResponse>(contentJson);
        }

        private string GetVrtToken(string username, string password)
        {
            _logger.LogDebug("Loggin in to VRT");
            var gigyaResponse = GetGigyaAuth(username, password);
            var url = new Uri("https://token.vrt.be");
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/json");
            webClient.Headers.Add("Referer", "https://www.vrt.be/vrtnu/");
            webClient.UploadString(url, JsonConvert.SerializeObject(new VrtLoginPayload
            {
                uid = gigyaResponse.UID,
                uidsig = gigyaResponse.UIDSignature,
                ts = gigyaResponse.signatureTimestamp,
                email = gigyaResponse.profile.email
            }));
            var xVrtToken = webClient.ResponseHeaders["set-cookie"].Split(';')[0].Replace("X-VRT-Token=", "");
            _logger.LogTrace("New X-VRT-TOKEN: " + xVrtToken);
            return xVrtToken;
        }

        private VrtPlayerTokenSet RefreshPlayerToken()
        {
            _logger.LogDebug("Refreshing player token");
            var url = "https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/tokens";
            var webClient = new WebClient();
            webClient.Headers.Add("cookie", $"X-VRT-Token={VrtToken};");
            var contentJson = webClient.UploadString(url, "");
            var tokenSet = JsonConvert.DeserializeObject<VrtPlayerTokenSet>(contentJson);
            _logger.LogTrace("New PlayerToken: " + tokenSet.vrtPlayerToken);
            return tokenSet;
        }
    }
}
