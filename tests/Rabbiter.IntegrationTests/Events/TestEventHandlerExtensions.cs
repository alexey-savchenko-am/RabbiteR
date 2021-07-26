namespace Rabbiter.IntegrationTests.Events
{
    using System.Threading.Tasks;

    public static class TestEventHandlerExtensions
    {
        public static async Task<int> WaitCompleted(this TestEventHandler handler, int expectedEventCount, int timeoutInSeconds = 10)
        {
            var t = timeoutInSeconds;
            var sec = 0;
            while (handler.ProcessedEventCount < expectedEventCount && timeoutInSeconds > 0)
            {
                await Task.Delay(1000);
                sec = t - --timeoutInSeconds;
            }

            return sec;
        }
    }
}
