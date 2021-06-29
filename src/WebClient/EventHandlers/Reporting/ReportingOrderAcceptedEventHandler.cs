namespace WebClient.EventHandlers
{
    using Rabbiter.Core.Abstractions.Events;
    using System.Linq;
    using System.Threading.Tasks;
    using WebClient.Data;
    using WebClient.Event;

    public class ReportingOrderAcceptedEventHandler
          : IEventHandler<OrderAcceptedEvent>
    {
        private readonly ShopContext _context;

        public ReportingOrderAcceptedEventHandler(ShopContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(IEventContainer<OrderAcceptedEvent> @event)
        {
            var orders = @event.Event.Products.GroupBy(k => k.ProductName, v => v.UnitPrice);
            
            foreach(var order in orders)
            {
                await _context.Orders.AddAsync(new Order
                {
                    Name = order.Key,
                    Price = order.ToList().Sum()
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
