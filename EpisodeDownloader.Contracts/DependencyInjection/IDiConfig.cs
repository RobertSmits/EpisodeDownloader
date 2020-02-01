using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Contracts.DependencyInjection
{
    public interface IDiConfig
    {
        void RegisterTypes(IServiceCollection services, IConfiguration configuration);
    }
}
