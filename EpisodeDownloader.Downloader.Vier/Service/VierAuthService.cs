using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EpisodeDownloader.Downloader.Vier.Service
{
    public class VierAuthService : IVierAuthService
    {
        const string AWS_REGION = "eu-west-1";
        const string CLIENT_ID = "6s1h851s8uplco5h6mqh1jac8m";
        const string USER_POOL_ID = "eu-west-1_dViSsKM5Y";

        private DateTime _expireDate;
        private AuthenticationResultType _authenticationResult;
        private readonly ILogger _logger;
        private readonly VierConfiguration _configuration;
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;

        public VierAuthService
            (
                ILogger<VierAuthService> logger,
                IOptionsMonitor<VierConfiguration> configMonitor
            )
        {
            _logger = logger;
            _configuration = configMonitor.CurrentValue;
            var cred = new AnonymousAWSCredentials();
            _provider = new AmazonCognitoIdentityProviderClient(cred, RegionEndpoint.EUWest1);
            _userPool = new CognitoUserPool(USER_POOL_ID, CLIENT_ID, _provider);
        }

        public string IdToken
        {
            get
            {
                if (_authenticationResult == null)
                {
                    _authenticationResult = LoginAsync(_configuration.Email, _configuration.Password).Result;
                    _expireDate = DateTime.Now.AddSeconds(_authenticationResult.ExpiresIn);
                }
                else if (_expireDate <= DateTime.Now)
                {
                    _authenticationResult = RefreshTokens(_configuration.RefreshToken).Result;
                    _expireDate = DateTime.Now.AddSeconds(_authenticationResult.ExpiresIn);
                }
                return _authenticationResult.IdToken;
            }
        }

        private async Task<AuthenticationResultType> LoginAsync(string username, string password)
        {
            _logger.LogDebug("Login attampt");
            _logger.LogTrace("Username: " + username);
            _logger.LogTrace("Password: " + password);
            CognitoUser user = new CognitoUser(username, CLIENT_ID, _userPool, _provider);
            AuthFlowResponse context = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest
            {
                Password = password
            }).ConfigureAwait(false);
            return context.AuthenticationResult;
        }

        private async Task<AuthenticationResultType> RefreshTokens(string refreshToken)
        {
            _logger.LogDebug("Refreshing login token");
            _logger.LogTrace("RefreshToken: " + refreshToken);
            CognitoUser user = new CognitoUser(null, CLIENT_ID, _userPool, _provider);
            user.SessionTokens = new CognitoUserSession(null, null, refreshToken, DateTime.Now, DateTime.Now.AddDays(3));
            AuthFlowResponse context = await user.StartWithRefreshTokenAuthAsync(new InitiateRefreshTokenAuthRequest
            {
                AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
            }).ConfigureAwait(false);
            return context.AuthenticationResult;
        }
    }
}
