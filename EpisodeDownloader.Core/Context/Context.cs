using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Core.Context
{
    public static class Context
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static ServiceProvider Container { get; set; }
    }
}
