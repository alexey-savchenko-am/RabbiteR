namespace Rabbiter.Core.Events.Listeners
{
    using Microsoft.Extensions.DependencyInjection;
    using Rabbiter.Core.Abstractions.EventProcessors;
    using Rabbiter.Core.Abstractions.Events;
    using System;
    using System.Threading.Tasks;

    public class EventProcessor<TEvent, THandler>
        : EventProcessor
        where TEvent : IEvent
        where THandler : IEventHandler<TEvent>
    {
        public EventProcessor(
            string eventGroupId, 
            string handleEventName,
            bool processFaultedEvent = false)
        {
            EventGroup = eventGroupId;
            EventName = handleEventName;
            ProcessFault = processFaultedEvent;
        }

        public override Task ProcessEventAsync(
            IEventContainer<IEvent> containedEvent, 
            IServiceProvider serviceProvider)
        {
            var handler = serviceProvider.GetRequiredService<THandler>();
            return handler.HandleAsync(containedEvent as IEventContainer<TEvent>);
        }

        public override bool IsProcess(Type eventType, string eventGroupId)
        {
            var handles = EventType == eventType;
            if (!string.IsNullOrEmpty(eventGroupId))
            {
                handles = handles && EventGroup.Equals(eventGroupId);
            }
            return handles;
        }

        public override Type HandlerType => typeof(THandler);
        public override Type EventType => typeof(TEvent);
    }



}
