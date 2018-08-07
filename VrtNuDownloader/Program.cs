using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var unityContainer = UnityConfig.Container;
            var downloader = unityContainer.Resolve<EpisodeDownloader>();
            var watchFile = unityContainer.Resolve<IFileService>().WatchFilePath;
            var showUrls = ReadLines(() => new FileStream(watchFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)).Select(x => new Uri(x));
            downloader.Run(showUrls);
        }

        private static IEnumerable<string> ReadLines(Func<Stream> streamProvider)
        {
            using (var stream = streamProvider())
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
