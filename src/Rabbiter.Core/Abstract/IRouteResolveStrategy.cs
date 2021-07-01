namespace Rabbiter.Core.Abstractions
{
    using System;
    public interface IRouteResolveStrategy
    {
        string Resolve(Type eventType, string queueName);
    }
}
