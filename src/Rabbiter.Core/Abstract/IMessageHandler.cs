namespace Rabbiter.Core.Abstractions
{
    using Rabbiter.Core.Abstractions.Messaging;

    public interface IMessageHandler
    {
        void Handle(IMessage consumeResult);
    }
}
