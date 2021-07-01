namespace Rabbiter.Core.Abstractions
{
    public interface IConsumerSubscription
    {
        public string ConsumerTag { get; }
        void Unsubscribe();
    }
}
