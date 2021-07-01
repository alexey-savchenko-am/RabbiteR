namespace Rabbiter.Core.Abstractions
{
    using Rabbiter.Core.Abstractions.Events;
    using System;

    public interface IPayloadTransformStrategy
    {
        TEvent ToEvent<TEvent>(string payload)
            where TEvent : IEvent;

        IEvent ToEvent(string payload, Type eventType);

        string ToMessage<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}
