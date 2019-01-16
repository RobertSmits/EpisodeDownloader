using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Context;
using VrtNuDownloader.Core.DependencyInjection;
using VrtNuDownloader.Core.Downloader;
using VrtNuDownloader.Downloader.Vrt.Service;

namespace VrtNuDownloader.Downloader.Vrt
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
