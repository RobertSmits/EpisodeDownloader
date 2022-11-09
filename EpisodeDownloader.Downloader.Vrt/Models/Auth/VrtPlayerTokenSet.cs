using System;

namespace EpisodeDownloader.Downloader.Vrt.Models.Auth;

public class VrtPlayerTokenSet
{
    public string vrtPlayerToken { get; set; }
    public DateTime expirationDate { get; set; }
}
