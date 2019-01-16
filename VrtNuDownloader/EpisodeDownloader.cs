using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Downloader;
using VrtNuDownloader.Core.Context;

namespace VrtNuDownloader
{
    public class EpisodeDownloader
    {
        public void Run(IEnumerable<Uri> ShowUrls)
        {
            var defaultDownloader = Context.Container.GetService<DefaultDownloader>();

            foreach (var showUrl in ShowUrls)
            {
                var handler = Context.Container
                    .GetServices<IDownloader>()
                    .FirstOrDefault(x => x.CanHandleUrl(showUrl))
                    ?? defaultDownloader;

                handler.Handle(showUrl);
            }
        }
    }
}
