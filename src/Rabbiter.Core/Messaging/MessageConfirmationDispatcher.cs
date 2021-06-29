namespace Rabbiter.Core.Messaging
{
    using Rabbiter.Core.Abstractions;
    using System;
    using System.Collections.Concurrent;

    public class MessageConfirmationDispatcher
    {
        private readonly ConcurrentDictionary<string, IMessageConfirmation> _messageToConfirmDictionary;

        public MessageConfirmationDispatcher()
        {
            _messageToConfirmDictionary = new ConcurrentDictionary<string, IMessageConfirmation>();
        }

        public IMessageConfirmation SubscribeOnConfirmation(string id, string exchange)
        {
            var confirmation = new MessageConfirmation(id, exchange);

            _messageToConfirmDictionary.AddOrUpdate(id, key => confirmation, (key, existing) => confirmation);

            return confirmation;
        }


        public void Unsubscribe(IMessageConfirmation confirmation)
        {
            _messageToConfirmDictionary.TryRemove(confirmation.MessageId, out var _);
        }

        internal void Approve(string messageId)
        {
            if (_messageToConfirmDictionary.TryRemove(messageId, out var confirmation))
                confirmation.Confirm();
        }

        internal void Reject(string messageId)
        {
            if (_messageToConfirmDictionary.TryRemove(messageId, out var confirmation))
                confirmation.Reject();
        }
    }
}


