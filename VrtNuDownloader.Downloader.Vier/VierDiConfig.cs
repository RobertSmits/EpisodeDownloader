using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Context;
using VrtNuDownloader.Core.DependencyInjection;
using VrtNuDownloader.Core.Downloader;
using VrtNuDownloader.Downloader.Vier.Service;

namespace VrtNuDownloader.Downloader.Vier
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
