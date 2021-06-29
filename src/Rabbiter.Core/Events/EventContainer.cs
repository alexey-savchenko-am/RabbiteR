namespace Rabbiter.Core.Events
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Messaging;
    using System;

    public class EventContainer<TEvent>
        : IEventContainer<TEvent>
        where TEvent : IEvent
    {
        public EventContainer(string id, string eventGroupId, IMessage message, Type eventType, string error, TEvent @event)
        {
            Id = id;
            EventGroupId = eventGroupId;
            Message = message;
            EventType = eventType;
            Error = error;
            Event = @event;
        }

        public string Id { get; }

        public string EventGroupId { get; }

        public Type EventType { get; }

        public TEvent Event { get; }

        public IMessage Message { get; }

        public string Error { get; }

        public override string ToString()
        {
            return $"{EventGroupId}:id-{Id}";
        }
    }
}
