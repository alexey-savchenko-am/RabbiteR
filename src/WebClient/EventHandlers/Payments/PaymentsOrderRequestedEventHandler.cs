namespace WebClient.EventHandlers
{
    using Rabbiter.Core.Abstractions.Events;
    using Rabbiter.Core.Abstractions.Publishers;
    using System;
    using System.Threading.Tasks;
    using WebClient.Event;

    public class PaymentsOrderRequestedEventHandler
        : IEventHandler<OrderRequestedEvent>
    {
        private readonly IEventPublisher _eventPublisher;
        private static readonly Random Random = new Random();

        public PaymentsOrderRequestedEventHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task HandleAsync(IEventContainer<OrderRequestedEvent> @event)
        {
            if (TryToPay())
                _eventPublisher.Publish(new OrderAcceptedEvent { Products = @event.Event.Products });
            else
                throw new InvalidOperationException($"{@event} Payment operation failed!");
        }

        // imitate payment operation
        private bool TryToPay()
        {
            var paymentFailed = Random.Next(1, 10) == 1;
            return !paymentFailed;
        }



    }
}
