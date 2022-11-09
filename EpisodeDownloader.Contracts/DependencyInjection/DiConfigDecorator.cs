using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Contracts.DependencyInjection;

public abstract class DiConfigDecorator : IDiConfig
{
    private readonly IDiConfig _diConfig;

    protected DiConfigDecorator(IDiConfig diConfig)
    {
        _diConfig = diConfig;
    }

    public virtual void RegisterTypes(IServiceCollection services, IConfiguration configuration)
    {
        _diConfig.RegisterTypes(services, configuration);
    }
}
