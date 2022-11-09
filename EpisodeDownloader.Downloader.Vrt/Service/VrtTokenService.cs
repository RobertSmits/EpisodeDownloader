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
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EpisodeDownloader.Downloader.Vrt.Service;

public class VrtTokenService : IVrtTokenService
{
    const string GIGYA_API_KEY = "3_qhEcPa5JGFROVwu5SWKqJ4mVOIkwlFNMSKwzPDAh8QZOtHqu6L4nD5Q7lk0eXOOG";
    const string GIGYA_LOGIN_URL = "https://accounts.vrt.be/accounts.login";
    const string VRT_LOGIN_URL = "https://login.vrt.be/perform_login?client_id=vrtnu-site";
    const string TOKEN_GATEWAY_URL = "https://token.vrt.be";
    const string USER_TOKEN_GATEWAY_URL = "https://token.vrt.be/vrtnuinitlogin?provider=site&destination=https://www.vrt.be/vrtnu/";
    const string ROAMING_TOKEN_GATEWAY_URL = "https://token.vrt.be/vrtnuinitloginEU?destination=https://www.vrt.be/vrtnu/";
    const string PLAYER_TOKEN_URL = "https://media-services-public.vrt.be/vualto-video-aggregator-web/rest/external/v2/tokens";


    const string PLAYER_INFO_JWT_KEY_ID = "0-0Fp51UZykfaiCJrfTE3+oMI8zvDteYfPtR+2n1R+z8w=";
    const string PLAYER_INFO_JWT_KEY = "2a9251d782700769fb856da5725daf38661874ca6f80ae7dc2b05ec1a81a24ae";

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
        var refreshtoken = GetCookieValue(response, "vrtlogin-rt");
        var accesstoken = GetCookieValue(response, "vrtlogin-at");
        var xVrtToken = GetCookieValue(response, "X-VRT-Token");

        _logger.LogTrace("New X-VRT-TOKEN: " + xVrtToken);
        return (xVrtToken, session);
    }

    private async Task<VrtPlayerTokenSet> RefreshPlayerTokenAsync()
    {
        var playerInfoToken = GeneratePlayerInfoJwt();

        _logger.LogDebug("Refreshing player token");
        var (token, session) = await GetVrtTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Post, PLAYER_TOKEN_URL);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            identityToken = token,
            playerInfo = playerInfoToken,
        }), System.Text.Encoding.UTF8, "application/json");
        using var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var tokenSet = JsonSerializer.Deserialize<VrtPlayerTokenSet>(content);
        _logger.LogTrace("New PlayerToken: " + tokenSet.vrtPlayerToken);
        ValidatePlayerToken(tokenSet.vrtPlayerToken);
        return tokenSet;
    }

    private string GeneratePlayerInfoJwt()
    {
        var playerInfoJson = new PlayerInfo
        {
            exp = DateTimeOffset.Now.ToUnixTimeSeconds() + 1000,

            // Needs to be desktop 
            platform = "desktop",

            app = new PlayerInfo.App
            {
                // Needs to be set to browser when platform is desktop
                type = "browser",

                // Actually not important
                name = "Firefox",
                version = "102.0"
            },
            device = "undefined (undefined)",

            // Actually not important
            os = new PlayerInfo.Os
            {
                // Actually not important
                name = "Windows",
                version = "10"
            },

            // Actually not important
            player = new PlayerInfo.Player
            {
                name = "VRT web player",
                version = "2.4.1-prod-2022-06-28T07:01:03"
            }
        };

        var encoder = new JwtEncoder(new HMACSHA256Algorithm(), new SystemTextSerializer(), new JwtBase64UrlEncoder());

        var jwtToken = encoder.Encode(
            new Dictionary<string, object>
            {
                { "kid", PLAYER_INFO_JWT_KEY_ID},
            },
            playerInfoJson,
            PLAYER_INFO_JWT_KEY);

        return jwtToken;
    }

    private void ValidatePlayerToken(string playerToken)
    {
        var decoder = new JwtDecoder(new SystemTextSerializer(), new JwtBase64UrlEncoder());
        var playerTokenObj = decoder.DecodeToObject<PlayerToken>(new JwtParts(playerToken), false);
        if (playerTokenObj.maxQuality != "HD")
        {
            _logger.LogWarning("Player token max quality: [{maxQuality}] differs from [HD] / playerToken [{playerToken}]", playerTokenObj.maxQuality, playerTokenObj);
        }
    }
}

internal record PlayerToken
{
    public long exp { get; init; }
    public string geoLocation { get; init; }
    public bool authenticated { get; init; }
    public string userStatus { get; init; }
    public string ageCategory { get; init; }
    public string maxQuality { get; init; }
}

internal record PlayerInfo
{
    public long exp { get; init; }
    public string platform { get; init; }
    public App app { get; init; }
    public string device { get; init; }
    public Os os { get; init; }
    public Player player { get; init; }

    public record App
    {
        public string type { get; init; }
        public string name { get; init; }
        public string version { get; init; }

    }

    public record Os
    {
        public string name { get; init; }
        public string version { get; init; }
    }

    public record Player
    {

        public string name { get; init; }
        public string version { get; init; }
    }
}
