using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using VrtNuDownloader.Core.Models;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Downloader.Vrt.Models.Auth;

namespace VrtNuDownloader.Downloader.Vrt.Service
{
    public class VrtTokenService : IVrtTokenService
    {
        const string TOKEN_FILE = ".vrt-cookies.yml";

        private VrtTokenSet _vrtTokenSet;
        private VrtPlayerTokenSet _vrtPlayerTokenSet;
        private readonly ILogger _logger;
        private readonly VrtConfiguration _configuration;
        private readonly IFileService _fileService;

        public VrtTokenService
            (
                ILogger<VrtTokenService> logger,
                IOptionsMonitor<VrtConfiguration> configMonitor,
                IFileService fileService
            )
        {
            _logger = logger;
            _configuration = configMonitor.CurrentValue;
            _fileService = fileService;
            LoadTokens();
        }

        private void LoadTokens()
        {
            var hasTokens = _fileService.CheckFileExists(TOKEN_FILE);
            if (hasTokens)
            {
                var tokenContainer = _fileService.ReadYamlFile<VrtTokenContainer>(TOKEN_FILE);
                _vrtTokenSet = tokenContainer.VrtTokenSet;
                _vrtPlayerTokenSet = tokenContainer.VrtPlayerTokenSet;
            }
        }

        public string VrtToken
        {
            get
            {
                if (_vrtTokenSet == null || new UnixTimeStamp(_vrtTokenSet.expiry) <= UnixTimeStamp.Now)
                {
                    _vrtTokenSet = RefreshTokenSet(_vrtTokenSet?.refreshtoken ?? _configuration.VrtLoginRt);
                    _fileService.WriteYamlFile(new VrtTokenContainer { VrtTokenSet = _vrtTokenSet, VrtPlayerTokenSet = _vrtPlayerTokenSet }, TOKEN_FILE);
                }
                return _vrtTokenSet.vrtnutoken;
            }
        }

        public string PlayerToken
        {
            get
            {
                if (_vrtPlayerTokenSet == null || _vrtPlayerTokenSet?.expirationDate <= DateTime.Now.ToUniversalTime())
                {
                    _vrtPlayerTokenSet = RefreshPlayerToken();
                    _fileService.WriteYamlFile(new VrtTokenContainer { VrtTokenSet = _vrtTokenSet, VrtPlayerTokenSet = _vrtPlayerTokenSet }, TOKEN_FILE);
                }
                return _vrtPlayerTokenSet.vrtPlayerToken;
            }
        }

        private VrtTokenSet RefreshTokenSet(string refreshToken)
        {
            _logger.LogDebug("Refreshing login token");
            _logger.LogTrace("RefreshToken: " + refreshToken);
            var url = "https://token.vrt.be/refreshtoken";
            var webClient = new WebClient();
            webClient.Headers.Add("cookie", $"vrtlogin-rt={refreshToken};");
            var contentJson = webClient.DownloadString(url);
            var tokenSet = JsonConvert.DeserializeObject<VrtTokenSet>(contentJson);
            _logger.LogTrace("New VrtToken: " + tokenSet.vrtnutoken);
            _logger.LogTrace("New RefreshToken: " + tokenSet.refreshtoken);
            return tokenSet;
        }

        private VrtPlayerTokenSet RefreshPlayerToken()
        {
            _logger.LogDebug("Refreshing player token");
            _logger.LogTrace("VrtToken: " + VrtToken);
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
