namespace Rabbiter.Core.Abstractions
{
    using RabbitMQ.Client;

    public interface IRmqExchangeFactory
    {
        void DeclareExchange(IModel channel, string exchangeName, string exchangeType = ExchangeType.Fanout, string bindToExchange = null);
    }
}
