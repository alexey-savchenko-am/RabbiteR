namespace Rabbiter.Core.Abstractions.Config
{
    using RabbitMQ.Client;

    public interface IConnectionProvider
    {
        IConnection Connect(RmqConnectionConfiguration connectionModel);
    }
}
