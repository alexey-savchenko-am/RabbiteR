namespace Rabbiter.Core.Abstractions
{
    using Rabbiter.Core.Abstractions.Handlers;
    using Rabbiter.Core.Abstractions.Messaging;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHandler
    {
        Task<MessageHandlerDelegate> GetMessageHandlerAsync(IModel channel, IDictionary<string, Type> handledEventDictionary);
    }
}
