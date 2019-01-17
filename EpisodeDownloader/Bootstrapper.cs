using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEscapades.Configuration.Yaml;
using EpisodeDownloader.Core.Context;
using EpisodeDownloader.Core.DependencyInjection;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace EpisodeDownloader
{
    public static class Bootstrapper
    {
        public static void Bootstrap()
        {
            LogManager.Configuration = NLogConfiguration.Value;
            Context.Configuration = configuration.Value;
            Context.Container = container.Value;
        }

        private static Lazy<LoggingConfiguration> NLogConfiguration = new Lazy<LoggingConfiguration>(() =>
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget("console")
            {
                Layout = @"${longdate}|${pad:padding=-10:${uppercase:${level}}}|${pad:fixedLength=true:alignmentOnTruncation=left:padding=-14:${logger:shortName=true}}|   |${message} ${exception}"
            };
            config.AddTarget(consoleTarget);
            config.AddRuleForAllLevels(consoleTarget);
            return config;
        });

        private static Lazy<IConfigurationRoot> configuration = new Lazy<IConfigurationRoot>(() =>
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
                env = "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile($"config.yml", optional: false, reloadOnChange: false)
                .AddYamlFile($"config.{env}.yml", optional: true, reloadOnChange: false);

            return builder.Build();
        });

        private static Lazy<ServiceProvider> container = new Lazy<ServiceProvider>(() =>
        {
            var serviceCollection = new ServiceCollection();
            var config = LoadAllDiConfigs();
            config.RegisterTypes(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        });

        private static IDiConfig LoadAllDiConfigs()
        {
            var decoratorTypes = Directory.GetFiles("./", "EpisodeDownloader.*.dll")
                                        .Select(Assembly.LoadFrom)
                                        .SelectMany(a => a.GetTypes())
                                        .Where(t => !t.IsInterface && !t.IsAbstract)
                                        .Where(t => t.IsSubclassOf(typeof(DiConfigDecorator)))
                                        .ToList();

            IDiConfig config = new DIConfig();
            config = decoratorTypes.Aggregate(config, (c, t) => (DiConfigDecorator)Activator.CreateInstance(t, new object[] { c }));
            return config;
        }
    }
}
