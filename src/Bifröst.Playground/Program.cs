using Bifröst.Publishers;
using Bifröst.Subscriptions;
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

        static void Main()
        {
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("creating service collection");
            var services = new ServiceCollection();

            services.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Trace);
                b.AddConsole();
            });

            services.AddTransient<AsyncActionSubscriptionFactory>();
            services.AddTransient<PublisherFactory>();
            services.AddSingleton<IBus, Bus>();

            services.AddSingleton<GetData>();
            services.AddSingleton<TransformData>();
            services.AddSingleton<SaveData>();

            logger.LogInformation("building service provider");
            var provider = services.BuildServiceProvider();

            EnableWorkers(logger, provider);

            Console.ReadLine();

            DisableWorkers(logger, provider);
        }

        private static void DisableWorkers(ILogger<Program> logger, ServiceProvider provider)
        {
            logger.LogInformation("starting get data module");
            provider.GetService<GetData>()
                .Disable();

            logger.LogInformation("starting transform data module");
            provider.GetService<TransformData>()
                .Disable();

            logger.LogInformation("starting save data module");
            provider.GetService<SaveData>()
                .Disable();

            logger.LogInformation("stopping bus");
            provider.GetService<IBus>()
                .Idle();
        }

        private static void EnableWorkers(ILogger<Program> logger, ServiceProvider provider)
        {
            logger.LogInformation("starting bus");
            provider.GetService<IBus>()
                .Run();

            logger.LogInformation("starting get data module");
            provider.GetService<GetData>()
                .Enable();

            logger.LogInformation("starting transform data module");
            provider.GetService<TransformData>()
                .Enable();

            logger.LogInformation("starting save data module");
            provider.GetService<SaveData>()
                .Enable();
        }
    }
}
