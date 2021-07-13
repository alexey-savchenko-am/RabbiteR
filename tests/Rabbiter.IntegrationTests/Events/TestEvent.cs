using Rabbiter.Core.Abstractions.Events;

namespace Rabbiter.IntegrationTests.Events
{
    public class TestEvent
        : IEvent
    {
        public int Id { get; set; }
    }
}
