namespace Rabbiter.IntegrationTests.Common
{
    using Rabbiter.Core.Abstractions.Events;
    using System;
    using System.Collections.Generic;

    public class Statistics
    {
        public Statistics()
        {
            ReceivedEvents = new List<IEventContainer<IEvent>>();
        }
        public int ReceivedMessageCount { get; set; } = 1;

        public DateTime FirstTime { get; set; } = DateTime.MaxValue;

        public DateTime LastTime { get; set; } = DateTime.MinValue;

        public List<IEventContainer<IEvent>> ReceivedEvents { get; set; }

        public TimeSpan EllapsedTime =>
            LastTime > FirstTime
                ? LastTime - FirstTime
                : TimeSpan.Zero;
    }
}
