using System.Collections.Generic;

namespace VrtNuDownloader.Downloader.Vier.Models.Auth
{
    public class CredentialRequestPayload
    {
        public string IdentityId { get; set; }
        public IDictionary<string, string> Logins { get; set; }

        public CredentialRequestPayload(string identityId, string login, string accessToken)
        {
            IdentityId = identityId;
            Logins = new Dictionary<string, string>();
            Logins.Add(login, accessToken);
        }
    }
}
