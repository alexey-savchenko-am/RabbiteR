namespace Rabbiter.Core.Abstractions.EventProcessors
{
    using Rabbiter.Core.Abstractions.Events;
    using System;
    using System.Threading.Tasks;


    public abstract class EventProcessor
    {
        public bool ProcessFault { get; protected set; }

        public string EventGroup { get; protected set; }

        public string EventName { get; protected set; }

        public abstract Type HandlerType { get; }

        public abstract Type EventType { get; }

        public abstract Task ProcessEventAsync(IEventContainer<IEvent> containedEvent, IServiceProvider serviceProvider);

        public abstract bool IsProcess(Type eventType, string eventGroupId);
    }


}
