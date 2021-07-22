namespace Rabbiter.Core.Abstract.Events
{
    using System.Threading.Tasks;

    public delegate Task<bool> EventListenerReconnectDelegate(string listenerId = "");
}
