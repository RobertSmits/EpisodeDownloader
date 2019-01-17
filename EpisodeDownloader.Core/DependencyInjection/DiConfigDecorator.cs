using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Core.DependencyInjection
{
    public abstract class DiConfigDecorator : IDiConfig
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
