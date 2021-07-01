namespace Rabbiter.Core.Abstractions
{
    using System;

    public interface IEventNameResolvingStrategy
    {
        string Resolve(Type eventType);

        string ResolveFault(Type eventType);
    }
}
