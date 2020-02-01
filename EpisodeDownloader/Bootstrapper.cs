using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EpisodeDownloader.Contracts.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace EpisodeDownloader
{
    public static class Bootstrapper
    {
        public static EpisodeDownloader BuildEpisodeDownloader()
        {
            LogManager.Configuration = BuildNLogConfiguration();
            var configuration = BuildConfiguration();
            var serviceProvider = BuildServiceProvider(configuration);
            return serviceProvider.GetRequiredService<EpisodeDownloader>();
        }

        private static LoggingConfiguration BuildNLogConfiguration()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget("console")
            {
                Layout = @"${longdate}|${pad:padding=-10:${uppercase:${level}}}|${pad:fixedLength=true:alignmentOnTruncation=left:padding=-14:${logger:shortName=true}}|   |${message} ${exception}"
            };
            config.AddTarget(consoleTarget);
            config.AddRuleForAllLevels(consoleTarget);
            return config;
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
                env = "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile($"config.yml", optional: false, reloadOnChange: false)
                .AddYamlFile($"config.{env}.yml", optional: true, reloadOnChange: false);

            return builder.Build();
        }

        private static IServiceProvider BuildServiceProvider(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            LoadAllDiConfigs().RegisterTypes(services, configuration);
            return services.BuildServiceProvider();
        }

        private static IDiConfig LoadAllDiConfigs()
        {
            var decoratorTypes = Directory.GetFiles("./", "EpisodeDownloader.*.dll")
                                   .Select(Assembly.LoadFrom)
                                   .SelectMany(a => a.GetTypes())
                                   .Where(t => !t.IsInterface && !t.IsAbstract && t.IsSubclassOf(typeof(DiConfigDecorator)));

            IDiConfig config = new DIConfig();
            return decoratorTypes.Aggregate(config, (c, t) => (DiConfigDecorator)Activator.CreateInstance(t, new[] { c }));
        }
    }
}
