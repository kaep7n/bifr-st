using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bifröst.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting...");

            Console.WriteLine("configuring dependencies");

            var services = new ServiceCollection();
            services.AddSingleton<IBus, Bus>();
            services.AddSingleton<Hub>();
            services.AddSingleton<GetDataWorker>();
            services.AddSingleton<TransformDataWorker>();
            services.AddSingleton<WriteToConsoleWorker>();


            Console.WriteLine("building service provider");
            var provider = services.BuildServiceProvider();

            Console.WriteLine("get hub");
            var hub = provider.GetService<Hub>();

            Console.ReadLine();
        }
    }
}
