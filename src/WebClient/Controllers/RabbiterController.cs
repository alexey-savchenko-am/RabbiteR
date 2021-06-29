namespace WebClient
{
    using AutoFixture;
    using Microsoft.AspNetCore.Mvc;
    using Rabbiter.Core.Abstractions.Publishers;
    using Rabbiter.Core.Events.Listeners;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WebClient.Event;

    public class RabbiterController
        : Controller
    {
        private readonly IEventPublisher _publisher;


        public RabbiterController(
            IEventPublisher publisher
         )
        {
            _publisher = publisher;
        }

        public async Task<ViewResult> Index()
        {
            return View();
        }

        [HttpPost("publish-order-requested-event")]
        public async Task PublishMessageAsync([FromBody]IList<Product> products)
        {
            var fixture = new Fixture();

            var p = !products.Any() 
                ? fixture.CreateMany<Product>().ToList() 
                : products.ToList();
            
            await _publisher.PublishAsync<OrderRequestedEvent>(new OrderRequestedEvent
            {
                Products = p
            });
        }


        [HttpPost("publish-random-order-requested-events")]
        public async Task PublishMessagesAsync([FromQuery]int count)
        {

            var fixture = new Fixture();

            var events = fixture.CreateMany<OrderRequestedEvent>(count);

           
           _publisher.PublishRange(events);
        }
    }
}
