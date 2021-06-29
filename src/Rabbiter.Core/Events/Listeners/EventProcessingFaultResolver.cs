namespace Rabbiter.Core.Events.Listeners
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Publishers;
    using System;
    using System.Threading.Tasks;

    public class EventProcessingFaultResolver
        : IEventProcessingFaultResolvingStrategy
    {
        private readonly ILogger<EventProcessingFaultResolver> _logger;
        private readonly IEventPublisher _eventPublisher;

        public EventProcessingFaultResolver(
            ILogger<EventProcessingFaultResolver> logger,
            IEventPublisher eventPublisher)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task ResolveFaultAsync<TEvent>(IEventContainer<TEvent> @event, Exception exception)
            where TEvent: IEvent
        {
            _logger.LogError($"{@event} error occurs while processing an event {@event.Id}");
            _logger.LogError($"{@event} about to send fault message...");
            _eventPublisher.PublishFault(@event.Event, exception.Message);
           
        }

        public async Task ResolveFaultAsync<TEvent>(IEventContainer<TEvent> @event, string exception) 
            where TEvent : IEvent
        {
            _logger.LogError($"{@event} error occurs while processing an event {@event.Id}");
            _logger.LogError($"{@event} about to send fault message...");
            _eventPublisher.PublishFault(@event.Event, exception);
        }
    }
}
