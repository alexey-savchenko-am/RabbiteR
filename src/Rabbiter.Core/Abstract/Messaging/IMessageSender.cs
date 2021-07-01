namespace Rabbiter.Core.Abstractions.Messaging
{
    using RabbitMQ.Client;
    using System.Threading.Tasks;

    public interface IMessageSender
    {
        Task SendMessageAsync(IModel channel, IMessage message);
    }
}
