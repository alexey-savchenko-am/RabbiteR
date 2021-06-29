namespace WebClient.EventHandlers
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions.Events;
    using System.Linq;
    using System.Threading.Tasks;
    using WebClient.Event;

    public class ShopFaultOrderRequestedEventHandler
    : IEventHandler<OrderRequestedEvent>
    {
        private readonly ILogger<ShopFaultOrderRequestedEventHandler> _logger;

        public ShopFaultOrderRequestedEventHandler(ILogger<ShopFaultOrderRequestedEventHandler> logger)
        {
            _logger = logger;
        }
        public Task HandleAsync(IEventContainer<OrderRequestedEvent> @event)
        {
            _logger.LogError("The following order was not processed:");

            foreach (var productGroup in @event.Event.Products.GroupBy(x => x.ProductName))
            {
                _logger.LogError($"{productGroup.Key} x{productGroup.Count()}");
            }

            return Task.CompletedTask;
        }
    }
}
