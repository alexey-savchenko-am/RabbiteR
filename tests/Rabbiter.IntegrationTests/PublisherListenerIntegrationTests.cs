namespace Rabbiter.IntegrationTests
{
    using AutoFixture;
    using Rabbiter.IntegrationTests.Common;
    using Rabbiter.IntegrationTests.Events;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

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
            var listener = _testFixture.TestEventListener;
            listener.Clear();


            var eventId = _testFixture.Fixture.Create<int>();

            _testFixture.Publisher.Publish(new TestEvent { Id = eventId });

            await WaitForEventProcessingCompletionAsync(listener, 1);

            Assert.Equal(1, listener.ProcessedEventCount);
        }

        [Fact]
        public async Task PublishMultipleMessagesToBrokerAndSuccessfullyReceiveAndProcessThem()
        {

            var listener = _testFixture.TestEventListener;

            listener.Clear();

            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
           
            _testFixture.Publisher.Publish(new TestEvent2 { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent2 { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent2 { Id = _testFixture.Fixture.Create<int>() });

            await WaitForEventProcessingCompletionAsync(listener, 6, 20);

            var statEvent1 = listener.EventStatistics(typeof(TestEvent));
            var statEvent2 = listener.EventStatistics(typeof(TestEvent2));

            Assert.Equal(6, listener.ProcessedEventCount);
            Assert.NotNull(statEvent1);
            Assert.NotNull(statEvent2);
            Assert.Equal(3, statEvent1.ReceivedMessageCount);
            Assert.Equal(3, statEvent2.ReceivedMessageCount);

        }


        private async Task WaitForEventProcessingCompletionAsync(
            TestEventListener listener, int expectedEventCount, int timeoutInSeconds = 10)
        {
            while (listener.ProcessedEventCount < expectedEventCount && timeoutInSeconds > 0)
            {
                await Task.Delay(1000);
                timeoutInSeconds--;
            }
        }

    }
}
