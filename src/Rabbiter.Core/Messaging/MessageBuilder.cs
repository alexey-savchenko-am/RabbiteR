namespace Rabbiter.Core.Messaging
{
    using Rabbiter.Core.Abstractions.Messaging;

    public class MessageBuilder
    {
        private IMessage _message;

        private MessageBuilder()
        { }


        public static MessageBuilder WithExchange(string exchange)
        {
            var builder = new MessageBuilder();
            var consumeResultBunch = new Message
            {
                Exchange = exchange
            };

            builder._message = consumeResultBunch;

            return builder;
        }

        public MessageBuilder WithTags(ulong deliveryTag, string consumerTag)
        {
            _message.DeliveryTag = deliveryTag;
            _message.ConsumerTag = consumerTag;
            return this;
        }

        public MessageBuilder WithPayload(string payload)
        {
            _message.Payload = payload;
            return this;
        }

        public MessageBuilder WithHeader(string name, string value)
        {
            _message.AddHeader(name, value);
            return this;
        }

        public IMessage Build()
        {
            return _message;
        }

    }
}
