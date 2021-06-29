namespace Rabbiter.Core.Messaging
{
    using Rabbiter.Core.Abstractions;
    using System;
    using System.Threading.Tasks;

    public class MessageConfirmation
        : IMessageConfirmation
    {
        private readonly string _exchangeName;
        
        public string MessageId { get; }

        private TaskCompletionSource<string> _completionSource =
            new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task ConfirmationTask => _completionSource.Task;

        public MessageConfirmation(string messageId, string exchangeName)
        {
            MessageId = messageId;
            _exchangeName = exchangeName;
        }



        public void Confirm()
        {
            _completionSource.TrySetResult(MessageId);
        }

        public void Reject()
        {
            _completionSource.TrySetException(new Exception($"message with id {MessageId} was rejected"));
        }
    }
}
