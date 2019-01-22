using System;

namespace EpisodeDownloader.Downloader.Vrt.Models.Auth
{
    public class GigyaAuthResponse
    {
        public Sessioninfo sessionInfo { get; set; }
        public bool newUser { get; set; }
        public string UID { get; set; }
        public string UIDSignature { get; set; }
        public string signatureTimestamp { get; set; }
        public string loginProvider { get; set; }
        public bool isRegistered { get; set; }
        public bool isActive { get; set; }
        public bool isVerified { get; set; }
        public string socialProviders { get; set; }
        public Profile profile { get; set; }
        public DateTime created { get; set; }
        public long createdTimestamp { get; set; }
        public DateTime lastLogin { get; set; }
        public long lastLoginTimestamp { get; set; }
        public DateTime lastUpdated { get; set; }
        public long lastUpdatedTimestamp { get; set; }
        public DateTime oldestDataUpdated { get; set; }
        public long oldestDataUpdatedTimestamp { get; set; }
        public DateTime registered { get; set; }
        public long registeredTimestamp { get; set; }
        public DateTime verified { get; set; }
        public long verifiedTimestamp { get; set; }
        public int statusCode { get; set; }
        public int errorCode { get; set; }
        public string statusReason { get; set; }
        public string callId { get; set; }
        public DateTime time { get; set; }
    }

    public class Sessioninfo
    {
        public string login_token { get; set; }
    }

    public class Profile
    {
        public string firstName { get; set; }
        public int birthYear { get; set; }
        public int birthMonth { get; set; }
        public int birthDay { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public int age { get; set; }
        public string email { get; set; }
    }
}
