using Bifröst.Playground.Events;
using Bifröst.Publishers;
using Bifröst.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bifröst.Playground
{
    public class TransformData
    {
        private readonly AsyncActionSubscription subscription;
        private readonly IPublisher publisher;
        private readonly ILogger logger;

        public TransformData(ILogger<TransformData> logger, AsyncActionSubscriptionFactory subscriptionFactory, PublisherFactory publisherFactory)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (subscriptionFactory is null)
            {
                throw new ArgumentNullException(nameof(subscriptionFactory));
            }

            if (publisherFactory is null)
            {
                throw new ArgumentNullException(nameof(publisherFactory));
            }

            var pattern = new PatternBuilder("playground")
               .With("data")
               .With("rng")
               .Build();

            this.subscription = subscriptionFactory.Create(pattern, this.TransformAsync);
            this.publisher = publisherFactory.Create();
            this.logger = logger;
        }

        public void Enable() => this.subscription.Enable();

        public void Disable() => this.subscription.Disable();

        private async Task TransformAsync(IEvent evt)
        {
            this.logger.LogDebug($"transform: received evt with topic {evt.Topic}");
            
            var topic = new TopicBuilder("playground")
                         .With("data")
                         .With("ascii")
                         .Build();

            if (!(evt is ValueEvent valueEvent))
            {
                this.logger.LogWarning($"transform: event is not from expected type ValueEvent");
                return;
            }

            this.logger.LogDebug("transform: transforming value to ascii");
            var ascii = Convert.ToChar(valueEvent.Value);

            this.logger.LogDebug($"transform: creating ascii event from transformed value {ascii}");
            var asciiEvent = new AsciiEvent(topic, ascii);
            
            this.logger.LogDebug("transform: enqueuing event");
            await this.publisher.PublishAsync(asciiEvent)
                .ConfigureAwait(false);
        }
    }
}
