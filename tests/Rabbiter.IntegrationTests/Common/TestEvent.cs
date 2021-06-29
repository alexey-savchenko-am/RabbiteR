using Rabbiter.Core.Abstractions.Events;

namespace Rabbiter.IntegrationTests.Common
{
    public class TestEvent
        : IEvent
    {
        public int Id { get; set; }
    }
}
