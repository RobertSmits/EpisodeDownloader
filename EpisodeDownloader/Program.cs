using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EpisodeDownloader.Core;
using EpisodeDownloader.Core.Context;

namespace EpisodeDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var logger = Context.Container.GetRequiredService<ILogger<Program>>();
            logger.LogDebug("Startup complete");
            var config = Context.Container.GetRequiredService<IOptions<Configuration>>().Value;
            if (config.WatchUrls == null) return;
            var showUrls = config.WatchUrls.Select(x => new Uri(x));
            Context.Container.GetService<EpisodeDownloader>().Run(showUrls);
        }
    }
}
