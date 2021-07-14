namespace Rabbiter.UnitTests.Messaging
{
    using System;
    using Xunit;
    using Rabbiter.Core.Messaging;
    using Rabbiter.UnitTests.Common;

    public class PayloadTransformatorTests
    {
        [Theory]
        [InlineData("{\"Id\":1,\"Name\":\"Paper\"}", typeof(TestEvent))]
        public void PayloadToEventAndToMessageTest(string payload, Type eventType)
        {
            var transformator = new JsonBasedPayloadTransformator();
            var @event = transformator.ToEvent(payload, eventType);
            Assert.NotNull(@event);
            var message = transformator.ToMessage(@event);
            Assert.NotNull(message);
            Assert.Equal(payload, message, ignoreCase: true, ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
            
        }
    }
}
