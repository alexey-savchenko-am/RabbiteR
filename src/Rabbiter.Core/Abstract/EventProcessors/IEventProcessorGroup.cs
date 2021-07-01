namespace Rabbiter.Core.Abstractions.EventProcessors
{
    using System;
    using System.Collections.Generic;

    public interface IEventProcessorGroup
    {
        string EventGroup { get; }

        IEnumerable<EventProcessor> ResolveEventProcessors(Type eventType, string eventGroupId, bool processFault = false);

        IDictionary<string, Type> ToHandledEventDictionary();

    }
}
