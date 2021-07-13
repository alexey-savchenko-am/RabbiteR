using Rabbiter.Core.Abstractions.Events;

namespace Rabbiter.IntegrationTests.Events
{
    public class TestEvent2
        : IEvent
    {
        public int Id { get; set; }
    }
}
