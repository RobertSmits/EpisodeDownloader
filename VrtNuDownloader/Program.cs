using System;
using System.Linq;
using VrtNuDownloader.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace VrtNuDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = DependencyInjectionConfig.Container;
            var downloader = container.GetService<EpisodeDownloader>();
            var config = container.GetRequiredService<IConfigService>();
            var showUrls = config.WatchUrls.Select(x => new Uri(x));
            downloader.Run(showUrls);
        }
    }
}
