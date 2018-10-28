using Newtonsoft.Json;
using System;
using System.Net;
using VrtNuDownloader.Core.Models;
using VrtNuDownloader.Core.Service.Config;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Downloader.Vrt.Models;

namespace VrtNuDownloader.Downloader.Vrt.Service
{
    public class VrtTokenService : IVrtTokenService
    {
        const string TOKEN_FILE = ".cookies.yml";

        private VrtTokenSet _vrtTokenSet;
        private VrtPlayerTokenSet _vrtPlayerTokenSet;
        private readonly IConfigService _configService;
        private readonly IFileService _fileService;

        public VrtTokenService(IConfigService configService, IFileService fileService)
        {
            _configService = configService;
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
                    _vrtTokenSet = RefreshTokenSet(_configService.Cookie ?? _vrtTokenSet?.refreshtoken);
                    _configService.Cookie = null;
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
            var url = "https://token.vrt.be/refreshtoken";
            var webClient = new WebClient();
            webClient.Headers.Add("cookie", $"vrtlogin-rt={refreshToken};");
            var contentJson = webClient.DownloadString(url);
            return JsonConvert.DeserializeObject<VrtTokenSet>(contentJson);
        }

        private VrtPlayerTokenSet RefreshPlayerToken()
        {
            var url = "https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/tokens";
            var webClient = new WebClient();
            webClient.Headers.Add("cookie", $"X-VRT-Token={VrtToken};");
            var contentJson = webClient.UploadString(url, "");
            return JsonConvert.DeserializeObject<VrtPlayerTokenSet>(contentJson);
        }
    }
}
