using Bifröst.Publishers;
using Bifröst.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bifröst.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBifröst(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<AsyncActionSubscriptionFactory>();
            services.AddTransient<PublisherFactory>();
            services.AddSingleton<IBus, Bus>();
        }
    }
}
