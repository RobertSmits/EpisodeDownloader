using Microsoft.Extensions.DependencyInjection;
using EpisodeDownloader.Core.Context;
using EpisodeDownloader.Core.DependencyInjection;
using EpisodeDownloader.Core.Downloader;
using EpisodeDownloader.Downloader.Vier.Service;

namespace EpisodeDownloader.Downloader.Vier
{
    public class VierDiConfig : DiConfigDecorator
    {
        public override void RegisterTypes(IServiceCollection serviceCollection)
        {
            base.RegisterTypes(serviceCollection);
            serviceCollection.AddTransient<IDownloader, VierDownloader>();
            serviceCollection.AddSingleton<IVierService, VierService>();
            serviceCollection.AddSingleton<IVierAuthService, VierAuthService>();
            serviceCollection.Configure<VierConfiguration>(Context.Configuration.GetSection(nameof(VierDownloader)));
        }

        public VierDiConfig(IDiConfig diConfig)
            : base(diConfig)
        {
        }
    }
}
