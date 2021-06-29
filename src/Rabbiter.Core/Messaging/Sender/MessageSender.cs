namespace Rabbiter.Core.Messaging.Sender
{
    using Rabbiter.Core.Abstractions.Messaging;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class MessageSender
        : IMessageSender
    {
        private readonly MessageConfirmationDispatcher _messageConfirmationDispatcher;

        public MessageSender(MessageConfirmationDispatcher messageConfirmationDispatcher)
        {
            _messageConfirmationDispatcher = messageConfirmationDispatcher;
        }

        public Task SendMessageAsync(IModel channel, IMessage message)
        {
            //var publishTag = channel.NextPublishSeqNo;

            var confirmation = _messageConfirmationDispatcher
                .SubscribeOnConfirmation(message.GetRequiredHeader(MessageHeaders.EventId), message.Exchange);

            try
            {
                channel.BasicPublish(
                    exchange: message.Exchange,
                    routingKey: "",
                    basicProperties: MakeProps(channel, message.Headers),
                    body: Encoding.UTF8.GetBytes(message.Payload)
                );
            }
            catch (Exception)
            {
                _messageConfirmationDispatcher.Unsubscribe(confirmation);
                throw;
            }

            return confirmation.ConfirmationTask;
        }

        private static IBasicProperties MakeProps(IModel channel, IDictionary<string, string> headers)
        {

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();

            foreach (var header in headers)
            {
                props.Headers.Add(header.Key, header.Value);
            }

            return props;
        }

    }
}
