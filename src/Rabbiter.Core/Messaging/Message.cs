namespace Rabbiter.Core.Messaging
{
    using Rabbiter.Core.Abstractions.Messaging;
    using System.Collections.Generic;
    


    public class Message
        : IMessage
    {
        public ulong DeliveryTag { get; set; }
        public string ConsumerTag { get; set; }
        public string Exchange { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string Payload { get; set; }

        public Message() { }

        public Message(
            ulong deliveryTag,
            string consumerTag,
            string exchange,
            IDictionary<string, string> headers,
            string payload
         )
        {
            DeliveryTag = deliveryTag;
            ConsumerTag = consumerTag;
            Exchange = exchange;
            Headers = headers;
            Payload = payload;
        }

        public string GetHeader(string name)
        {
            if (!Headers.ContainsKey(name))
                return null;

            return Headers[name];
        }


        public string GetRequiredHeader(string name)
        {
            if (!Headers.ContainsKey(name))
                throw new KeyNotFoundException(name);

            return Headers[name];
        }

        public void AddHeader(string name, string value)
        {
            if (Headers == null)
            {
                Headers = new Dictionary<string, string>();
            }

            Headers[name] = value;
        }

        public override string ToString()
        {
            return $"{Exchange}:tg-{DeliveryTag}";
        }
    }
}
