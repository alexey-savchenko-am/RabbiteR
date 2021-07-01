namespace Rabbiter.Core.Abstractions.Events
{
    using Rabbiter.Core.Abstractions.Messaging;
    using System;

    public interface IEventContainer<out TEvent>
        where TEvent : IEvent
    {
        string Id { get; }

        public string EventGroupId { get; }

        public string Error { get; }

        Type EventType { get; }

        TEvent Event { get; }

        IMessage Message { get; }

    }
}
