namespace VrtNuDownloader.Downloader.Vier {
    internal static class Extensiond
    {
        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }
            return false;
        }
    }
}
