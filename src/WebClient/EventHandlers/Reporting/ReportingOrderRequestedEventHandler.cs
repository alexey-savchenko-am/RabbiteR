namespace WebClient.EventHandlers
{
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions.Events;
    using System.Linq;
    using System.Threading.Tasks;
    using WebClient.Event;

    public class ReportingOrderRequestedEventHandler
        : IEventHandler<OrderRequestedEvent>
    {
        private readonly ILogger<ReportingOrderRequestedEventHandler> _logger;

        public ReportingOrderRequestedEventHandler(ILogger<ReportingOrderRequestedEventHandler> logger)
        {
            _logger = logger;
        }
        public Task HandleAsync(IEventContainer<OrderRequestedEvent> @event)
        {
            _logger.LogInformation($"Requested orders total sum {@event.Event.Products.Sum(p => p.UnitPrice)}$");

            return Task.CompletedTask;
        }
    }
}
