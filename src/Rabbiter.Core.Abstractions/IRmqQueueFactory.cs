namespace Rabbiter.Core.Abstractions
{
    using RabbitMQ.Client;

    public interface IRmqQueueFactory
    {
        string DeclareQueue(IModel channel, string exchangeName, string queueName, string routingKey = "");
    }
}
