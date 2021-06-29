namespace Rabbiter.Core.Events.Listeners
{
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.EventProcessors;
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Events;
    using System.Collections.Generic;
    using System.Linq;

    public class EventProcessorBuilder
    {

        private readonly IEventNameResolvingStrategy _eventNameResolver;

        private readonly IList<EventProcessor> _eventProcessors
            = new List<EventProcessor>();

        private readonly string _eventGroupId;

        private EventProcessorBuilder(string eventGroupId,
            IEventNameResolvingStrategy eventNameResolvingStrategy)
        {
            _eventGroupId = eventGroupId;
            _eventNameResolver = eventNameResolvingStrategy;
        }

        public static EventProcessorBuilder WithEventGroup(string eventGroupId)
        {
            return WithEventGroupAndNameResolver(eventGroupId, new EventNameResolver());
        }

        public static EventProcessorBuilder WithEventGroupAndNameResolver(string eventGroupId,
            IEventNameResolvingStrategy eventNameResolvingStrategy)
        {
            return new EventProcessorBuilder(eventGroupId, eventNameResolvingStrategy);
        }


        public EventProcessorBuilder SubscribeOn<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {

            _eventProcessors.Add(new EventProcessor<TEvent, TEventHandler>
                (_eventGroupId, _eventNameResolver.Resolve(typeof(TEvent))));

            return this;
        }


        public EventProcessorBuilder OnFault<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {

            _eventProcessors.Add(new EventProcessor<TEvent, TEventHandler>
                (_eventGroupId, _eventNameResolver.ResolveFault(typeof(TEvent)), processFaultedEvent: true));

            return this;
        }

        public EventProcessorGroup Build()
        {

            var eventProcessorGroup = new EventProcessorGroup(_eventGroupId);

            foreach (var iProcessor in _eventProcessors)
            {
                eventProcessorGroup.Add(iProcessor);
            }

            return eventProcessorGroup;

        }

    }
}
