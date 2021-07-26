namespace Rabbiter.Core
{
    using RabbitMQ.Client;

    public static class ChannelExtensions
    {
        public static void DeclareExchange(
            this IModel channel,
            string exchangeName,
            string exchangeType = ExchangeType.Fanout,
            string bindToExchange = null)
        {
            channel.ExchangeDeclare(
                exchange: exchangeName,
                durable: true,
                type: exchangeType
            );

            if (bindToExchange != null)
            {
                channel.ExchangeBind(bindToExchange, exchangeName, "");
            }

        }


        public static string DeclareQueue(this IModel channel, string exchangeName, string queueName, string routingKey = "")
        {
            var queue = channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            
            channel.QueueBind(queueName, exchangeName, routingKey: routingKey);

            return queue.QueueName;
        }

    }


}
