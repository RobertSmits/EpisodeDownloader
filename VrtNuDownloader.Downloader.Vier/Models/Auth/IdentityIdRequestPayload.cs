using System.Collections.Generic;

namespace VrtNuDownloader.Downloader.Vier.Models.Auth
{
    public class IdentityIdRequestPayload
    {
        public string IdentityPoolId { get; set; }
        public IDictionary<string, string> Logins { get; set; }

        public IdentityIdRequestPayload (string poolId, string login, string accessToken)
        {
            IdentityPoolId = poolId;
            Logins = new Dictionary<string, string>();
            Logins.Add(login, accessToken);
        }
    }
}
