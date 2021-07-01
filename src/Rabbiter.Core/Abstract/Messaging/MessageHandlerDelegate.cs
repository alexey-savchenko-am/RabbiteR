namespace Rabbiter.Core.Abstractions.Handlers
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Messaging;
    using System;
    using System.Threading.Tasks;

    public delegate Task<IEventContainer<IEvent>> MessageHandlerDelegate(IMessage message);
}
