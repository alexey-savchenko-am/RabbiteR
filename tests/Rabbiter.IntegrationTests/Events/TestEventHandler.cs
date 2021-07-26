namespace Rabbiter.IntegrationTests.Events
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.IntegrationTests.Common;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class TestEventHandler
        : IEventHandler<TestEvent>
        , IEventHandler<TestEvent2>
    {

        volatile int _count = 0;

        public int ProcessedEventCount => _count;

        private ConcurrentDictionary<Type, Statistics> _statisticsDistionary;
            
        public TestEventHandler()
        {
            _statisticsDistionary = new ConcurrentDictionary<Type, Statistics>();
        }

       
        public Statistics EventStatistics(Type eventType) {

           if(_statisticsDistionary.TryGetValue(eventType, out var val))
           {
               return val;
           }

            return default(Statistics);
        }

        public void Clear()
        {
            _count = 0;
            foreach (var s in _statisticsDistionary.Values)
            {
                s.ReceivedMessageCount = 0;
                s.FirstTime = DateTime.MaxValue;
                s.LastTime = DateTime.MinValue;
                s.ReceivedEvents = new List<IEventContainer<IEvent>>();
            }
        }

        public Task HandleAsync(IEventContainer<TestEvent> @event)
        {

            ProcessEvent(typeof(TestEvent), @event);
            return Task.CompletedTask;
        }

        public Task HandleAsync(IEventContainer<TestEvent2> @event)
        {
            ProcessEvent(typeof(TestEvent2), @event);
            return Task.CompletedTask;
        }

        private Statistics ProcessEvent<TEvent>(Type eventType, IEventContainer<TEvent> @event)
            where TEvent: IEvent
        {
            Interlocked.Increment(ref _count);

            var e = (IEventContainer<IEvent>)@event;

            var newStat = new Statistics();
            newStat.ReceivedEvents.Add(e);

            return _statisticsDistionary.AddOrUpdate(eventType,
               newStat,
               (t, s) =>
               {
                   DateTime now = DateTime.Now;

                   if (now < s.FirstTime) s.FirstTime = now;
                   if (now > s.LastTime) s.LastTime = now;

                   s.ReceivedEvents.Add(e);
                   s.ReceivedMessageCount++;

                   return s;
               });

        }


    }
}
