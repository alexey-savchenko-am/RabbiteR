namespace Rabbiter.Core.Abstractions.Messaging
{
    using System.Collections.Generic;

    public interface IMessage
    {
        ulong DeliveryTag { get; set; }
        string ConsumerTag { get; set; }
        string Exchange { get; set; }
        IDictionary<string, string> Headers { get; set; }
        string Payload { get; set; }

        void AddHeader(string name, string value);

        string GetHeader(string name);
        string GetRequiredHeader(string name);
    }
}
