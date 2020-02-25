using Bifröst.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    class Program
    {
        private static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(b =>
        {
            b.SetMinimumLevel(LogLevel.Trace);
            b.AddConsole();
        });

        static async Task Main()
        {
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("adding services");
            var services = new ServiceCollection();

            ConfigureServices(services);

            logger.LogInformation("building service provider");
            var provider = services.BuildServiceProvider();

            await EnableWorkersAsync(logger, provider)
                .ConfigureAwait(false);

            Console.ReadLine();

            await DisableWorkersAsync(logger, provider)
                .ConfigureAwait(false);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Trace);
                b.AddDebug();
                b.AddConsole();
            });

            services.AddBifröst();

            services.AddSingleton<GenerateData>();
            services.AddSingleton<TransformData>();
            services.AddSingleton<SaveData>();
        }

        private static async Task DisableWorkersAsync(ILogger<Program> logger, ServiceProvider provider)
        {
            logger.LogInformation("starting get data module");
            provider.GetService<GenerateData>()
                .Disable();

            logger.LogInformation("starting transform data module");
            await provider.GetService<TransformData>()
                .DisableAsync()
                .ConfigureAwait(false);

            logger.LogInformation("starting save data module");
            await provider.GetService<SaveData>()
                .DisableAsync()
                .ConfigureAwait(false);

            logger.LogInformation("stopping bus");
            await provider.GetService<IBus>()
                .IdleAsync()
                .ConfigureAwait(false);
        }

        private static async Task EnableWorkersAsync(ILogger<Program> logger, ServiceProvider provider)
        {
            logger.LogInformation("starting bus");
            await provider.GetService<IBus>()
                .RunAsync()
                .ConfigureAwait(false);

            logger.LogInformation("starting get data module");
            provider.GetService<GenerateData>()
                .Enable();

            logger.LogInformation("starting transform data module");
            await provider.GetService<TransformData>()
                .EnableAsync()
                .ConfigureAwait(false);

            logger.LogInformation("starting save data module");
            await provider.GetService<SaveData>()
                .EnableAsync()
                .ConfigureAwait(false);
        }
    }
}
