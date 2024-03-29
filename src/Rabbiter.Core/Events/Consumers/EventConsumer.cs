﻿namespace Rabbiter.Core.Events.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core;
    using Rabbiter.Core.Abstract.Events;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Messaging.Consumers;
    using RabbitMQ.Client;

    public class EventConsumer
        : IEventConsumer, IDisposable
    {
        private IConnection _connection;
        private readonly IRmqResourceManager _rmqResourceManager;
        private readonly IHandler _messageHandlerFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<EventConsumer> _logger;

        private readonly Dictionary<string, ConsumerSubscription> _consumers
            = new Dictionary<string, ConsumerSubscription>();

        public EventConsumer(
            ILoggerFactory loggerFactory,
            IConnection connection,
            IRmqResourceManager rmqResourceManager,
            IHandler messageHandlerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<EventConsumer>();
            _connection = connection;
            _rmqResourceManager = rmqResourceManager;
            _messageHandlerFactory = messageHandlerFactory;
        }


        public async Task<IConsumerSubscription> SubscribeAsync(
            string listenerId,
            string eventGroupId,
            IDictionary<string, Type> handledEventDictionary,
            EventHandlerDelegate eventHandler,
            EventListenerReconnectDelegate onReconnect)
        {
            // open channel 
            var channel = _rmqResourceManager.OpenChannel(
                _connection,
                eventGroupId,
                handledEventDictionary.Select(h => h.Key)
            );

            var rmqConsumer = new RmqMessageConsumer(
                queueName: eventGroupId,
                logger: _loggerFactory.CreateLogger<RmqMessageConsumer>(),
                channel: channel,
                messageHandler:
                    await _messageHandlerFactory.GetMessageHandlerAsync(channel, handledEventDictionary));


            rmqConsumer.MessageProcessed += (s, e) => eventHandler(e);
            rmqConsumer.ConsumerCancelled += (s, e) => onReconnect();

            channel.BasicConsume(
             queue: eventGroupId,
             autoAck: false,
             consumerTag: listenerId,
             noLocal: true,
             exclusive: false,
             arguments: null,
             consumer: rmqConsumer
            );

            var subscription = new ConsumerSubscription(
                listenerId,
                rmqConsumer,
                () =>
                {
                    rmqConsumer.MessageProcessed -= (s, e) => eventHandler(e);
                    rmqConsumer.ConsumerCancelled -= (s, e) => onReconnect();
                  
                    // this command will also close channel 
                    rmqConsumer.Dispose();
                    _consumers.Remove(listenerId);
                });

            _consumers.Add(listenerId, subscription);

            return subscription;
        }

        public void Dispose()
        {
            foreach (var consumer in _consumers)
            {
                consumer.Value.Unsubscribe();
                _consumers.Remove(consumer.Key);
            }

            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
