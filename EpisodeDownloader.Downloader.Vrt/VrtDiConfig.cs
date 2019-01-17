using Microsoft.Extensions.DependencyInjection;
using EpisodeDownloader.Core.Context;
using EpisodeDownloader.Core.DependencyInjection;
using EpisodeDownloader.Core.Downloader;
using EpisodeDownloader.Downloader.Vrt.Service;

namespace EpisodeDownloader.Downloader.Vrt
{
    public class VrtDiConfig : DiConfigDecorator
    {
        public override void RegisterTypes(IServiceCollection serviceCollection)
        {
            base.RegisterTypes(serviceCollection);
            serviceCollection.AddTransient<IDownloader, VrtDownloader>();
            serviceCollection.AddSingleton<IVrtNuService, VrtNuService>();
            serviceCollection.AddSingleton<IVrtTokenService, VrtTokenService>();
            serviceCollection.Configure<VrtConfiguration>(Context.Configuration.GetSection(nameof(VrtDownloader)));
        }

        public VrtDiConfig(IDiConfig diConfig)
            : base(diConfig)
        {
        }
    }
}
