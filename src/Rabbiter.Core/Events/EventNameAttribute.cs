namespace Rabbiter.Core.Events
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute
        : Attribute
    {
        public string EventName { get; private set; }
 
        public EventNameAttribute(string name)
        {
            EventName = name;
        }
    }
}
