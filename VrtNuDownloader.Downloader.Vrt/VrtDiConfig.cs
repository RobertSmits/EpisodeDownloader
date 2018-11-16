using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.DependencyInjection;
using VrtNuDownloader.Core.Interfaces;
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
        }

        public VrtDiConfig(IDiConfig diConfig)
            : base(diConfig)
        {
        }
    }
}
