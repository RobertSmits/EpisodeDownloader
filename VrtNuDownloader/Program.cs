using System;
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
            var config = unityContainer.Resolve<IConfigService>();
            var showUrls = config.WatchUrls.Select(x => new Uri(x));
            downloader.Run(showUrls);
        }
    }
}
