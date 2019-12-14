using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Core.DependencyInjection
{
    public interface IDiConfig
    {
        void RegisterTypes(IServiceCollection serviceCollection);
    }
}
