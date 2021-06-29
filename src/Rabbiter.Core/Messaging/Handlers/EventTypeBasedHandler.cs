namespace Rabbiter.Core.Messaging.Handlers
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Handlers;
    using Rabbiter.Core.Abstractions.Messaging;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class EventTypeBasedHandler
        : IHandler
    {
        private readonly ILogger<EventTypeBasedHandler> _logger;
        private readonly IHandler _messageHandler;
        private readonly ConcurrentDictionary<string, MessageHandlerDelegate>
            _messageHandlers = new ConcurrentDictionary<string, MessageHandlerDelegate>();

        public EventTypeBasedHandler(ILogger<EventTypeBasedHandler> logger, IHandler messageHandler)
        {
            _logger = logger;
            _messageHandler = messageHandler;
        }

        public async Task<MessageHandlerDelegate> GetMessageHandlerAsync(IModel channel, IDictionary<string, Type> handledEventDictionary)
        {
            return async message =>
            {
                var eventName = message.GetHeader(MessageHeaders.EventName);

                if (!_messageHandlers.TryGetValue(eventName, out var handler))
                {
                    // request new handler for each event type
                    var messageHandler = await _messageHandler
                        .GetMessageHandlerAsync(channel, handledEventDictionary);

                    var newOrAddedHandler = _messageHandlers.GetOrAdd(eventName, messageHandler);

                    if (newOrAddedHandler != handler)
                    {
                        handler = newOrAddedHandler;
                    }
                }

                return await handler.Invoke(message);
            };
        }
    }
}
