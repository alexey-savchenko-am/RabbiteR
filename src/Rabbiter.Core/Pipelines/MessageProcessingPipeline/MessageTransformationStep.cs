namespace Rabbiter.Core.Pipelines.MessageProcessingPipeline
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Messaging;
    using Rabbiter.Core.Events;
    using Rabbiter.Core.Messaging;
    using Rabbiter.Core.Pipelines.Builders;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MessageTransformationStep
        : IPipelineStep<MessageWithHandlers, IEventContainer<IEvent>>
    {
        private readonly ILogger<MessageTransformationStep> _logger;
        private readonly IPayloadTransformStrategy _payloadTransformStrategy;

        public MessageTransformationStep(
            ILogger<MessageTransformationStep> logger,
            IPayloadTransformStrategy payloadTransformStrategy)
        {
            _logger = logger;
            _payloadTransformStrategy = payloadTransformStrategy;
        }

        public async Task<IEventContainer<IEvent>> InvokeAsync(MessageWithHandlers input, Func<MessageWithHandlers, Task<IEventContainer<IEvent>>> next)
        {
            return ToEvent(input.Message, input.HandledEventDictionary);
        }

        private IEventContainer<IEvent> ToEvent(IMessage message, IDictionary<string, Type> handledEventDictionary)
        {

            string eventName = message.GetRequiredHeader(MessageHeaders.EventName);
            string id = message.GetHeader(MessageHeaders.EventId);
            string eventGroup = message.GetHeader(MessageHeaders.EventGroup) ?? message.Exchange;
            string eventError = message.GetHeader(MessageHeaders.EventErrorMessage);

            if (eventName == null || !handledEventDictionary.TryGetValue(eventName, out var eventType))
            {
                var errorMessage = $"Handler for event {eventName} not found";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            var @event = _payloadTransformStrategy.ToEvent(message.Payload, eventType);
            
            var containedEvent 
                = typeof(EventContainer<>)
                    .MakeGenericType(eventType)
                    .GetInstance(id, eventGroup, message, eventType, eventError, @event);
            
            _logger.LogInformation($"{@event} event constructed and ready to be processed");

            return (IEventContainer<IEvent>)containedEvent;
        }


    }
}
