﻿namespace Rabbiter.Core.Messaging.Consumers
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Handlers;
    using Rabbiter.Core.Messaging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class RmqMessageConsumer
        : IBasicConsumer, IDisposable
    {
        public IModel Model => _channel;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;
        public event EventHandler<IEventContainer<IEvent>> MessageProcessed;
        public string QueueName => _queueName;

        private readonly string _queueName;
        private readonly ILogger<RmqMessageConsumer> _logger;
        private readonly MessageHandlerDelegate _messageHandler;
        private readonly IModel _channel;

        public RmqMessageConsumer(
            string queueName,
            ILogger<RmqMessageConsumer> logger,
            IModel channel,
            MessageHandlerDelegate messageHandler)
        {
            _queueName = queueName;
            _logger = logger;
            _channel = channel;
            _messageHandler = messageHandler;
        }

        //
        // Summary:
        //     Called when the consumer is cancelled for reasons other than by a basicCancel:
        //     e.g. the queue has been deleted (either by this channel or by any other channel).
        //     See RabbitMQ.Client.IBasicConsumer.HandleBasicCancelOk(System.String) for notification
        //     of consumer cancellation due to basicCancel
        //
        // Parameters:
        //   consumerTag:
        //     Consumer tag this consumer is registered.
        public void HandleBasicCancel(string consumerTag)
        {
            _logger.LogError(
                $"Channel for queue {_queueName} has been closed. " +
                $"Perhaps queue has been corrupted or removed."
            );
            ConsumerCancelled.Invoke(this, new ConsumerEventArgs(new string[] { consumerTag }));
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
            _logger.LogInformation($"consumer {consumerTag} stopped");
        }

        //
        // Summary:
        //     Called upon successful registration of the consumer with the broker.
        //
        // Parameters:
        //   consumerTag:
        //     Consumer tag this consumer is registered.
        public void HandleBasicConsumeOk(string consumerTag)
        {
            _logger.LogInformation($"consumer {consumerTag} successfully registered");
        }

        //
        // Summary:
        //     Called each time a message arrives for this consumer.
        //
        // Remarks:
        //     Does nothing with the passed in information. Note that in particular, some delivered
        //     messages may require acknowledgement via RabbitMQ.Client.IModel.BasicAck(System.UInt64,System.Boolean).
        //     The implementation of this method in this class does NOT acknowledge such messages.
        public void HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;

            var messageBuilder =
                MessageBuilder
                    .WithExchange(exchange)
                    .WithTags(deliveryTag, consumerTag)
                    .WithPayload(Encoding.UTF8.GetString(body.ToArray()));

            if (properties.Headers != null)
            {
                foreach (var header in properties.Headers)
                {
                    var val = (byte[])header.Value;

                    messageBuilder.WithHeader(header.Key, Encoding.UTF8.GetString(val));
                }

            }

            var messageHandlerTask
                = _messageHandler.Invoke(messageBuilder.Build());

            messageHandlerTask.ContinueWith(
               r => OnFailed(Model, deliveryTag),
               TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously)
            .ConfigureAwait(false);

            messageHandlerTask.ContinueWith(
               r => OnSuccess(r.Result, Model, deliveryTag),
               TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously)
            .ConfigureAwait(false);

        }

        private static void OnFailed(IModel channel, ulong deliveryTag)
        {
            channel.BasicNack(deliveryTag, false, false);
        }

        private void OnSuccess(IEventContainer<IEvent> @event, IModel channel, ulong deliveryTag)
        {
            MessageProcessed.Invoke(this, @event);
            channel.BasicAck(deliveryTag, false);
        }


        //
        // Summary:
        //     Called when the model shuts down.
        //
        // Parameters:
        //   model:
        //     Common AMQP model.
        //
        //   reason:
        //     Information about the reason why a particular model, session, or connection was
        //     destroyed.
        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            _logger.LogError($"Channel for queue {_queueName} has been closed. {reason}.");
            ConsumerCancelled.Invoke(this, new ConsumerEventArgs(new string[] { }));
        }


        public void Dispose()
        {
            
            if (!Model.IsClosed)
            {
                Model.Abort();
                Model.Dispose();
            }
        }

    }
}
