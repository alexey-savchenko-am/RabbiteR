namespace Rabbiter.IntegrationTests.Events
{
    using Rabbiter.Core.Abstractions.Events;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TestEventListener
        : IEventHandler<TestEvent>
        , IEventHandler<TestEvent2>
    {
        
        public int ProcessedEventCount => 
            _statisticsDistionary.Aggregate(0, (total, stat) =>
                total + stat.Value.ReceivedMessageCount);

        private ConcurrentDictionary<Type, Statistics> _statisticsDistionary;
            
        public TestEventListener()
        {
            _statisticsDistionary = new ConcurrentDictionary<Type, Statistics>();
        }

        public class Statistics
        {
            public Statistics(int messageCount, DateTime lastReceivedDate)
            {
                ReceivedMessageCount = messageCount;
                LastMessageReceivedDateTime = lastReceivedDate;
                ReceivedEvents = new List<IEventContainer<IEvent>>();
            }
            public int ReceivedMessageCount { get; set; }

            public DateTime LastMessageReceivedDateTime { get; set; }

            public List<IEventContainer<IEvent>> ReceivedEvents { get; set; }
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
            var e = (IEventContainer<IEvent>)@event;

            var newStat = new Statistics(1, DateTime.Now);
            newStat.ReceivedEvents.Add(e);
            return _statisticsDistionary.AddOrUpdate(typeof(TestEvent),
               newStat,
               (t, s) =>
               {
                   DateTime now = DateTime.Now;
                   if (now < s.LastMessageReceivedDateTime)
                       s.LastMessageReceivedDateTime = now;

                   s.ReceivedEvents.Add(e);
                   s.ReceivedMessageCount++;

                   return s;
               });

        }


    }
}
