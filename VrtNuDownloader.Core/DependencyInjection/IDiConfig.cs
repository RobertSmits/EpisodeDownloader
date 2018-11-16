using Microsoft.Extensions.DependencyInjection;

namespace VrtNuDownloader.Core.DependencyInjection
{
    public interface IDiConfig
    {
        void RegisterTypes(IServiceCollection serviceCollection);
    }
}
