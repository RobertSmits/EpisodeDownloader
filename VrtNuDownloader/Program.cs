using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Service.Config;

namespace VrtNuDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = DependencyInjectionConfig.Container;
            var showUrls = container.GetRequiredService<IConfigService>().WatchUrls.Select(x => new Uri(x));
            container.GetService<EpisodeDownloader>().Run(showUrls);
        }
    }
}
