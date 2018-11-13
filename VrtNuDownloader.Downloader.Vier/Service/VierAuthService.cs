using Newtonsoft.Json;
using System;
using System.Net;
using VrtNuDownloader.Core.Service.Config;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Downloader.Vier.Models.Auth;

namespace VrtNuDownloader.Downloader.Vier.Service
{
    public class VierAuthService : IVierAuthService
    {
        const string AWS_REGION = "eu-west-1";
        const string CLIENT_ID = "6s1h851s8uplco5h6mqh1jac8m";
        const string USER_POOL_ID = "eu-west-1:8b7eb22c-cf61-43d5-a624-04b494867234";

        private DateTime _expireDate;
        private AuthenticationResult _authenticationResult;
        private readonly IConfigService _configService;

        public VierAuthService(IConfigService configService)
        {
            _configService = configService;
        }

        public string IdToken
        {
            get
            {
                RefreshIfNeeded();
                return _authenticationResult.IdToken;
            }
        }

        public string AccessToken
        {
            get
            {
                RefreshIfNeeded();
                return _authenticationResult.AccessToken;
            }
        }

        private void RefreshIfNeeded()
        {
            if (_authenticationResult == null || _expireDate <= DateTime.Now)
            {
                _authenticationResult = RefreshTokens(_configService.VierRefreshToken);
                _expireDate = DateTime.Now.AddSeconds(_authenticationResult.ExpiresIn);
            }
        }

        private AuthenticationResult RefreshTokens(string refreshToken)
        {
            var url = $"https://cognito-idp.{AWS_REGION}.amazonaws.com/";
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-amz-json-1.1");
            webClient.Headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");
            var payload = JsonConvert.SerializeObject(new RefreshRequestPayload(CLIENT_ID, refreshToken));
            var resultJson = webClient.UploadString(url, payload);
            return JsonConvert.DeserializeObject<Authentication>(resultJson).AuthenticationResult;
        }

        private string GetIdentityId()
        {
            var url = $"https://cognito-identity.{AWS_REGION}.amazonaws.com/";
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-amz-json-1.1");
            webClient.Headers.Add("X-Amz-Target", "AWSCognitoIdentityService.GetId");
            var payload = JsonConvert.SerializeObject(new IdentityIdRequestPayload(USER_POOL_ID, $"cognito-idp.{AWS_REGION}.amazonaws.com/eu-west-1_dViSsKM5Y", IdToken));
            var resultJson = webClient.UploadString(url, payload);
            return JsonConvert.DeserializeObject<IdentityIdResult>(resultJson).IdentityId;
        }

        public Credentials GetCredentials()
        {
            var url = $"https://cognito-identity.{AWS_REGION}.amazonaws.com/";
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-amz-json-1.1");
            webClient.Headers.Add("X-Amz-Target", "AWSCognitoIdentityService.GetCredentialsForIdentity");
            var payload = JsonConvert.SerializeObject(new CredentialRequestPayload(GetIdentityId(), $"cognito-idp.{AWS_REGION}.amazonaws.com/eu-west-1_dViSsKM5Y", IdToken));
            var resultJson = webClient.UploadString(url, payload);
            return JsonConvert.DeserializeObject<CredentialResult>(resultJson).Credentials;
        }
    }
}
