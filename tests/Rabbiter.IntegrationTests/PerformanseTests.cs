namespace Rabbiter.IntegrationTests
{
    using Rabbiter.IntegrationTests.Common;
    using Rabbiter.IntegrationTests.Events;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("SequentialIntegrationTests")]
    public class PerformanseTests
    {
        [Theory]
        [InlineData(1000, 30)]  // 1000 messages within 30 sec
        [InlineData(10000, 60)] // 10 000 messages within 60 sec
        //[InlineData(100000, 120)] // 10 0000 messages within 120 sec
        public async Task PublishAndProcessRangeOfMessagesForCertainTime(int messageCount, int expectedProcessTimeInSec)
        {
            using var fixture = new IntegrationTestFixture();
            
            var handler = fixture.TestEventHandler;
  
            // publish range of messages to bus
            var publishTasks
                = Enumerable
                    .Range(0, messageCount)
                    .Select(x => Task.Run(() => { fixture.Publisher.Publish(new TestEvent { Id = x }); }));

            var maxWaitTime = expectedProcessTimeInSec + 40;

            // await while messages will be processed
            await Task.WhenAll(publishTasks);

            var ellapsedSeconds = await handler.WaitCompleted(messageCount, maxWaitTime);

            var statistics = handler.EventStatistics(typeof(TestEvent));
            var processedTimeInSec = statistics.EllapsedTime.TotalSeconds;

            Trace.WriteLine($"{messageCount} has been processed for {processedTimeInSec} seconds ({expectedProcessTimeInSec} expected)");
            Assert.Equal(messageCount, handler.ProcessedEventCount);
            Assert.True(processedTimeInSec < maxWaitTime);
        }
    }
}
