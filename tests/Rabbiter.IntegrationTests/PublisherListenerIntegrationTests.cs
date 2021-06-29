namespace Rabbiter.IntegrationTests
{
    using AutoFixture;
    using Rabbiter.IntegrationTests.Common;
    using System;
    using System.Threading;
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
        public void PublishOneMessageToBrokerAndSuccessfullyReceiveAndProcessOne()
        {
            var eventId = _testFixture.Fixture.Create<int>();

            _testFixture.Publisher.Publish(new TestEvent { Id = eventId });

            Assert.True(WaitUntilEventsProcessingCompleted(_testFixture.TestEventListener, 1));
        }

        [Fact]
        public void PublishMultipleMessagesToBrokerAndSuccessfullyReceiveAndProcessThem()
        {
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });
            _testFixture.Publisher.Publish(new TestEvent { Id = _testFixture.Fixture.Create<int>() });

     
            Assert.True(WaitUntilEventsProcessingCompleted(_testFixture.TestEventListener, 5,20));
        }


        private bool WaitUntilEventsProcessingCompleted(
            TestEventListener listener, int expectedEventCount, int timeoutInSeconds = 10)
        {
            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds)).Token;

            while (!ct.IsCancellationRequested)
            {
                if (listener.ProcessedEventCount != expectedEventCount)
                    continue;
                else
                    return true;
            }

            return false;
           
        }

     }
}
