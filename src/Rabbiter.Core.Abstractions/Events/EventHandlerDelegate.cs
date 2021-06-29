namespace Rabbiter.Core.Abstractions.Events
{
    using System;
    using System.Threading.Tasks;

    public delegate Task EventHandlerDelegate(IEventContainer<IEvent> @event);
}
