namespace EpisodeDownloader.Downloader.Vrt.Models.Auth;

public class VrtLoginPayload
{
    public string UID { get; set; }
    public string UIDSignature { get; set; }
    public string signatureTimestamp { get; set; }
    public string client_id { get; set; }
    public string _csrf { get; set; }
}
