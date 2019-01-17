using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using EpisodeDownloader.Downloader.Vier.Models.Auth;

namespace EpisodeDownloader.Downloader.Vier.Service
{
    public class VierAuthService : IVierAuthService
    {
        const string AWS_REGION = "eu-west-1";
        const string CLIENT_ID = "6s1h851s8uplco5h6mqh1jac8m";
        const string USER_POOL_ID = "eu-west-1:8b7eb22c-cf61-43d5-a624-04b494867234";

        private DateTime _expireDate;
        private AuthenticationResult _authenticationResult;
        private readonly ILogger _logger;
        private readonly VierConfiguration _configService;

        public VierAuthService
            (
                ILogger<VierAuthService> logger,
                IOptionsMonitor<VierConfiguration> configMonitor
            )
        {
            _logger = logger;
            _configService = configMonitor.CurrentValue;
        }

        public string IdToken
        {
            get
            {
                if (_authenticationResult == null || _expireDate <= DateTime.Now)
                {
                    _authenticationResult = RefreshTokens(_configService.RefreshToken);
                    _expireDate = DateTime.Now.AddSeconds(_authenticationResult.ExpiresIn);
                }
                return _authenticationResult.IdToken;
            }
        }

        private AuthenticationResult RefreshTokens(string refreshToken)
        {
            _logger.LogDebug("Refreshing login token");
            _logger.LogTrace("RefreshToken: " + refreshToken);
            var url = $"https://cognito-idp.{AWS_REGION}.amazonaws.com/";
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-amz-json-1.1");
            webClient.Headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");
            var payload = JsonConvert.SerializeObject(new RefreshRequestPayload(CLIENT_ID, refreshToken));
            var resultJson = webClient.UploadString(url, payload);
            var authResult = JsonConvert.DeserializeObject<Authentication>(resultJson).AuthenticationResult;
            _logger.LogTrace("IdToken: " + authResult.IdToken);
            return authResult;
        }
    }
}
