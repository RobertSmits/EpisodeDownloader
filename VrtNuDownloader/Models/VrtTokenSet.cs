﻿namespace VrtNuDownloader.Models
{
    public class VrtTokenSet
    {
        public string vrtnutoken { get; set; }
        public string vrtprofiletoken { get; set; }
        public string refreshtoken { get; set; }
        public long expiry { get; set; }
    }
}
