namespace Rabbiter.IntegrationTests
{
    using AutoFixture;
    using Rabbiter.IntegrationTests.Common;
    using Rabbiter.IntegrationTests.Events;
    using System.Threading.Tasks;
    using Xunit;


    [Collection("SequentialIntegrationTests")]
    public class PublisherListenerIntegrationTests
        : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _testFixture;
        public PublisherListenerIntegrationTests(IntegrationTestFixture integrationTestFixture)
        {
            _testFixture = integrationTestFixture;
        }

        [Fact]
        public async Task PublishOneMessageToBrokerAndSuccessfullyReceiveAndProcessOne()
        {
            var handler = _testFixture.TestEventHandler;
            handler.Clear();

            var eventId = _testFixture.Fixture.Create<int>();

            _testFixture.Publisher.Publish(new TestEvent { Id = eventId });

            await handler.WaitCompleted(1);

            Assert.Equal(1, handler.ProcessedEventCount);
        }

        [Fact]
        public async Task PublishMultipleMessagesToBrokerAndSuccessfullyReceiveAndProcessThem()
        {

            var handler = _testFixture.TestEventHandler;

            handler.Clear();

            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
           
            _testFixture.Publisher.Publish(new TestEvent2 { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent2 { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent2 { Id = _testFixture.Fixture.Create<int>() });

            await handler.WaitCompleted(6, 20);

            var statEvent1 = handler.EventStatistics(typeof(TestEvent));
            var statEvent2 = handler.EventStatistics(typeof(TestEvent2));

            Assert.Equal(6, handler.ProcessedEventCount);
            Assert.NotNull(statEvent1);
            Assert.NotNull(statEvent2);
            Assert.Equal(3, statEvent1.ReceivedMessageCount);
            Assert.Equal(3, statEvent2.ReceivedMessageCount);

        }
    }
}
