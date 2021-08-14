using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EpisodeDownloader.Downloader.Vrt.Extensions;
using EpisodeDownloader.Downloader.Vrt.Models;
using EpisodeDownloader.Downloader.Vrt.Models.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EpisodeDownloader.Downloader.Vrt.Service
{
    public class VrtTokenService : IVrtTokenService
    {
        const string GIGYA_API_KEY = "3_qhEcPa5JGFROVwu5SWKqJ4mVOIkwlFNMSKwzPDAh8QZOtHqu6L4nD5Q7lk0eXOOG";
        const string GIGYA_LOGIN_URL = "https://accounts.vrt.be/accounts.login";
        const string VRT_LOGIN_URL = "https://login.vrt.be/perform_login?client_id=vrtnu-site";
        const string TOKEN_GATEWAY_URL = "https://token.vrt.be";
        const string USER_TOKEN_GATEWAY_URL = "https://token.vrt.be/vrtnuinitlogin?provider=site&destination=https://www.vrt.be/vrtnu/";
        const string ROAMING_TOKEN_GATEWAY_URL = "https://token.vrt.be/vrtnuinitloginEU?destination=https://www.vrt.be/vrtnu/";
        const string PLAYER_TOKEN_URL = "https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v1/tokens";

        private VrtPlayerTokenSet _vrtPlayerTokenSet;
        private readonly ILogger _logger;
        private readonly VrtConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public VrtTokenService
            (
                ILogger<VrtTokenService> logger,
                IOptionsMonitor<VrtConfiguration> configMonitor
            )
        {
            _logger = logger;
            _configuration = configMonitor.CurrentValue;
            _httpClient = new HttpClient();
        }

        public Task<(string, string)> GetVrtTokenAsync()
        {
            return GetVrtTokenAsync(_configuration.Email, _configuration.Password);
        }

        public async Task<string> GetPlayerTokenAsync()
        {
            if (_vrtPlayerTokenSet == null || _vrtPlayerTokenSet?.expirationDate <= DateTime.Now.ToUniversalTime())
                _vrtPlayerTokenSet = await RefreshPlayerTokenAsync();
            return _vrtPlayerTokenSet.vrtPlayerToken;
        }

        private async Task<GigyaAuthResponse> GetGigyaAuthAsync(string username, string password)
        {
            _logger.LogDebug("Logging in to Gigya");
            _logger.LogTrace("Username: " + username);
            _logger.LogTrace("Password: " + password);
            var url = new Uri(GIGYA_LOGIN_URL)
                .AddParameter("APIKey", GIGYA_API_KEY)
                .AddParameter("targetEnv", "jssdk")
                .AddParameter("loginID", username)
                .AddParameter("password", password)
                .AddParameter("sessionExpiration", "-2")
                .AddParameter("authMode", "cookie");
            var contentJson = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<GigyaAuthResponse>(contentJson);
        }

        private string GetCookieValue(HttpResponseMessage response, string name)
        {
            return response.Headers.GetValues("set-cookie").FirstOrDefault(x => x.StartsWith(name + "=")).Split(';').First().Replace(name + "=", "");
        }

        private async Task<(string, string)> GetVrtTokenAsync(string username, string password)
        {
            _logger.LogDebug("Loggin in to VRT");
            var gigyaResponse = await GetGigyaAuthAsync(username, password);
            var cookieContainer = new CookieContainer();
            var cl = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = cookieContainer
            });
            var response = await cl.GetAsync(USER_TOKEN_GATEWAY_URL);
            var state = GetCookieValue(response, "state");
            response = await cl.GetAsync(response.Headers.Location);
            var session = GetCookieValue(response, "SESSION");
            var xsrf = GetCookieValue(response, "OIDCXSRF");


            var request = new HttpRequestMessage(HttpMethod.Post, VRT_LOGIN_URL);
            request.Headers.Add("Cookie", $"SESSION={session}; OIDCXSRF={xsrf}");
            request.Content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("UID", gigyaResponse.UID),
                    new KeyValuePair<string, string>("UIDSignature", gigyaResponse.UIDSignature),
                    new KeyValuePair<string, string>("signatureTimestamp", gigyaResponse.signatureTimestamp),
                    new KeyValuePair<string, string>("_csrf", xsrf),
                });

            response = await cl.SendAsync(request);

            request = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            request.Headers.Add("Cookie", $"SESSION={session};OIDCXSRF={xsrf}");
            response = await cl.SendAsync(request);

            request = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            request.Headers.Add("Cookie", $"state={state}");
            response = await cl.SendAsync(request);
            var xVrtToken = GetCookieValue(response, "X-VRT-Token");

            //cookies = response.Headers.GetValues("set-cookie").Aggregate((prev, current) => prev + ";" + current);
            //var xVrtToken = cookies.Split(';').First(x => x.StartsWith("X-VRT-Token")).Replace("X-VRT-Token=", "");
            _logger.LogTrace("New X-VRT-TOKEN: " + xVrtToken);
            return (xVrtToken, session);
        }

        private async Task<VrtPlayerTokenSet> RefreshPlayerTokenAsync()
        {
            _logger.LogDebug("Refreshing player token");
            var (token, session) = await GetVrtTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Post, PLAYER_TOKEN_URL);
            request.Headers.Add("cookie", $"X-VRT-Token={token};SESSION={session}");
            request.Content = new StringContent(JsonSerializer.Serialize(new { identityToken = token }), System.Text.Encoding.UTF8, "application/json");
            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var tokenSet = JsonSerializer.Deserialize<VrtPlayerTokenSet>(content);
            _logger.LogTrace("New PlayerToken: " + tokenSet.vrtPlayerToken);
            return tokenSet;
        }
    }
}
