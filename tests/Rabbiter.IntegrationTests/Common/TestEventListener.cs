namespace Rabbiter.IntegrationTests.Common
{
    using Rabbiter.Core.Abstractions.Events;
    using System.Threading.Tasks;

    public class TestEventListener
        : IEventHandler<TestEvent>
    {

        public int ProcessedEventCount { get; set; }

        public async Task HandleAsync(IEventContainer<TestEvent> @event)
        {
            ProcessedEventCount++;
        }
    }
}
