using System;

namespace VrtNuDownloader.Downloader.Vrt.Models.Auth
{
    public class VrtPlayerTokenSet
    {
        public string vrtPlayerToken { get; set; }
        public DateTime expirationDate { get; set; }
    }
}
