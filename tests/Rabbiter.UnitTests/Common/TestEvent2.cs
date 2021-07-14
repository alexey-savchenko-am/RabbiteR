namespace Rabbiter.UnitTests.Common
{
    using Newtonsoft.Json;
    using Rabbiter.Core.Abstractions.Events;
    using System;

    class TestEvent2 : IEvent
    {
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
