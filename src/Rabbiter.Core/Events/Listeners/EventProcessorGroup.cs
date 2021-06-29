namespace Rabbiter.Core.Events.Listeners
{
    using Rabbiter.Core.Abstractions.EventProcessors;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class EventProcessorGroup
        : Collection<EventProcessor>, IEventProcessorGroup
    {
        public string EventGroup { get; }

        public EventProcessorGroup(string eventGroup)
        {
            EventGroup = eventGroup;
        }

        public IEnumerable<EventProcessor> ResolveEventProcessors(Type eventType, string eventGroupId, bool processFault = false)
        {
            foreach (var processor in Items)
            {
                if (processor.IsProcess(eventType, eventGroupId) && processor.ProcessFault == processFault)
                    yield return processor;
            }
        }

        public IDictionary<string, Type> ToHandledEventDictionary()
        {
            var result = new Dictionary<string, Type>();
            foreach (var item in Items)
            {
                result.TryAdd(item.EventName, item.EventType);
            }

            return result;
        }

    }
}
