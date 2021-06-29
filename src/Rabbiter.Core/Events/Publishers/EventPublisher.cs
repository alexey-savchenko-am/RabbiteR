namespace Rabbiter.Core.Publishers
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Messaging;
    using Rabbiter.Core.Abstractions.Publishers;
    using Rabbiter.Core.Messaging;
    using Rabbiter.Core.Messaging.Sender;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class EventPublisher
        : IEventPublisher
    {

        private readonly ILogger<IEventPublisher> _logger;
        private readonly IMessageSender _messageSender;
        private readonly IConnection _connection;
        private readonly IEventNameResolvingStrategy _eventNameResolvingStrategy;
        private readonly IPayloadTransformStrategy _payloadTransformator;
        private IModel _channel;
        private readonly QueueManager<IMessage> _messageManager;
                

        public EventPublisher(
            ILogger<IEventPublisher> logger,
            IMessageSender messageSender,
            IConnection connection,
            IEventNameResolvingStrategy eventNameResolvingStrategy,
            IPayloadTransformStrategy payloadTransformator)
        {
            _logger = logger;
            _messageSender = messageSender;
            _connection = connection;
            _eventNameResolvingStrategy = eventNameResolvingStrategy;
            _payloadTransformator = payloadTransformator;
            _channel = _connection.CreateModel();

            _messageManager
                = new QueueManager<IMessage>(m => _messageSender.SendMessageAsync(_channel, m));

        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
            => PublishEventInternalAsync(@event, false, string.Empty, string.Empty);

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
            => PublishEventInternal(@event, false, string.Empty, string.Empty);

        public void Publish<TEvent>(TEvent @event, string eventGroupId) where TEvent : IEvent
            => PublishEventInternal(@event, false, eventGroupId, string.Empty);

        public void Publish<TEvent>(TEvent @event, string eventGroupId, string id) where TEvent : IEvent
            => PublishEventInternal(@event, false, eventGroupId, id);


        public Task PublishFaultAsync<TEvent>(TEvent @event, string errorReason) where TEvent : IEvent
           => PublishEventInternalAsync(@event, true, string.Empty, string.Empty, errorReason);

        public void PublishFault<TEvent>(TEvent @event, string errorReason) where TEvent : IEvent
            => PublishEventInternal(@event, true, string.Empty, string.Empty, errorReason);

        public void PublishFault<TEvent>(TEvent @event, string eventGroupId, string errorReason) where TEvent : IEvent
            => PublishEventInternal(@event, true, eventGroupId, string.Empty, errorReason);

        public void PublishFault<TEvent>(TEvent @event, string eventGroupId, string id, string errorReason) where TEvent : IEvent
            => PublishEventInternal(@event, true, eventGroupId, id, errorReason);

        public void PublishRange<TEvent>(IEnumerable<TEvent> events, string eventGroupId = "")
            where TEvent : IEvent
        {
            Parallel.ForEach(events, (@event) =>
            {
                Publish(@event, eventGroupId);
            });
        }


        private Task PublishEventInternalAsync<TEvent>(TEvent @event, bool isFaultEvent, string eventGroupId, string id, string errorReason = "")
            where TEvent : IEvent
        {
            var message = CreateMessage(@event, isFaultEvent, eventGroupId, id, errorReason);

            _logger.LogInformation($"{message} About to send message tid: {Thread.CurrentThread.ManagedThreadId}");

            return _messageSender.SendMessageAsync(_channel, message);
        }

        private void PublishEventInternal<TEvent>(TEvent @event, bool isFaultEvent, string eventGroupId, string id, string errorReason = "")
             where TEvent : IEvent
        {

            var message = CreateMessage(@event, isFaultEvent, eventGroupId, id, errorReason);

            _logger.LogInformation($"{message} About to send message tid: {Thread.CurrentThread.ManagedThreadId}");

            _messageManager.Enqueue(message);
        }

        private IMessage CreateMessage<TEvent>(TEvent @event, bool isFaultEvent, string eventGroupId, string id, string errorReason = "")
            where TEvent : IEvent
        {
            var eventType = @event.GetType();
            var exchangeName
                = isFaultEvent
                    ? _eventNameResolvingStrategy.ResolveFault(eventType)
                    : _eventNameResolvingStrategy.Resolve(eventType);

            return BuildMessage(@event, exchangeName, eventGroupId, id, errorReason);
        }


        private IMessage BuildMessage<TEvent>(TEvent @event, string exchange, string eventGroupId, string id, string errorReason )
            where TEvent : IEvent
        {
            var builder 
                = MessageBuilder
                    .WithExchange(exchange)
                    .WithHeader(MessageHeaders.EventId, string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id)
                    .WithHeader(MessageHeaders.EventGroup, eventGroupId)
                    .WithHeader(MessageHeaders.EventName, exchange)
                    .WithHeader(MessageHeaders.PublishDateTime, DateTime.UtcNow.ToString("o"))
                    .WithPayload(_payloadTransformator.ToMessage(@event));

            if (!string.IsNullOrEmpty(errorReason)) 
                builder.WithHeader(MessageHeaders.EventErrorMessage, errorReason);

            return builder.Build();
        }

    }
}
