namespace Rabbiter.Core.Abstractions.Messaging
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Handlers;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMessageHandlerFactory
    {
        Task<MessageHandlerDelegate> CreateMessageHandlerAsync(
                 IModel channel,
                 IDictionary<string, Type> handledEventDictionary,
                 EventHandlerDelegate eventHandler);
    }
}
