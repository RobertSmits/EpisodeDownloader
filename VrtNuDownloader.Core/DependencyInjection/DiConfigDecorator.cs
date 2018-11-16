using Microsoft.Extensions.DependencyInjection;

namespace VrtNuDownloader.Core.DependencyInjection
{
    public class DiConfigDecorator : IDiConfig
    {
        private IDiConfig _diConfig;

        public virtual void RegisterTypes(IServiceCollection serviceCollection)
        {
            _diConfig.RegisterTypes(serviceCollection);
        }

        public DiConfigDecorator(IDiConfig diConfig)
        {
            _diConfig = diConfig;
        }
    }
}
