using Bifröst.Extensions;
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

            logger.LogInformation("adding services");
            var services = new ServiceCollection();

            ConfigureServices(services);

            logger.LogInformation("building service provider");
            var provider = services.BuildServiceProvider();

            EnableWorkers(logger, provider);

            Console.ReadLine();

            DisableWorkers(logger, provider);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Trace);
                //b.AddDebug();
                b.AddConsole();
            });

            services.AddBifröst();

            services.AddSingleton<GenerateData>();
            services.AddSingleton<TransformData>();
            services.AddSingleton<SaveData>();
        }

        private static void DisableWorkers(ILogger<Program> logger, ServiceProvider provider)
        {
            logger.LogInformation("starting get data module");
            provider.GetService<GenerateData>()
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
            provider.GetService<GenerateData>()
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
