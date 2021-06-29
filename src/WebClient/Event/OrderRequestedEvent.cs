namespace WebClient.Event
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Events;
    using System.Collections.Generic;

    [EventName("order-requested-event")]
    public class OrderRequestedEvent
        : IEvent
    {
        public IList<Product> Products { get; set; }
    }
}
