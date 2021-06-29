namespace WebClient.Event
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Events;
    using System.Collections.Generic;

    [EventName("order-accepted-event")]
    public class OrderAcceptedEvent
        : IEvent
    {
        public IList<Product> Products { get; set; }
    }
}
