using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Bifröst.Playground
{
    class Program
    {
        private static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(b =>
        {
            b.SetMinimumLevel(LogLevel.Trace);
            b.AddConsole();
        });

        static void Main(string[] args)
        {
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("creating service collection");
            var services = new ServiceCollection();

            services.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Information);
                b.AddConsole();
            });
            services.AddSingleton<IBus, Bus>();

            services.AddSingleton<GetData>();
            services.AddSingleton<TransformData>();
            services.AddSingleton<SaveData>();

            logger.LogInformation("building service provider");
            var provider = services.BuildServiceProvider();

            StartModules(logger, provider);

            Console.ReadLine();

            StopModules(logger, provider);
        }

        private static void StopModules(ILogger<Program> logger, ServiceProvider provider)
        {
            logger.LogInformation("starting get data module");
            provider.GetService<GetData>()
                .Stop();

            logger.LogInformation("starting transform data module");
            provider.GetService<TransformData>()
                .Stop();

            logger.LogInformation("starting save data module");
            provider.GetService<SaveData>()
                .Stop();

            logger.LogInformation("stopping bus");
            provider.GetService<IBus>()
                .Stop();
        }

        private static void StartModules(ILogger<Program> logger, ServiceProvider provider)
        {
            logger.LogInformation("starting bus");
            provider.GetService<IBus>()
                .Start();

            logger.LogInformation("starting get data module");
            provider.GetService<GetData>()
                .Start();

            logger.LogInformation("starting transform data module");
            provider.GetService<TransformData>()
                .Start();

            logger.LogInformation("starting save data module");
            provider.GetService<SaveData>()
                .Start();
        }
    }
}
