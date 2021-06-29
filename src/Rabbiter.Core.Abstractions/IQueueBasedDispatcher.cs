namespace Rabbiter.Core.Abstractions
{
    using System;
    using System.Threading.Tasks;

    public interface IQueueBasedDispatcher<T>
    {
        void Enqueue(T item);
    }
}
