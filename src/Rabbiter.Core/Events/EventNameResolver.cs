namespace Rabbiter.Core.Events
{
    using Rabbiter.Core.Abstractions;
    using System;
    using System.Text.RegularExpressions;

    public class EventNameResolver
        : IEventNameResolvingStrategy
    {
        public string Resolve(Type eventType)
        {
            var attributes = eventType
                .GetCustomAttributes(typeof(EventNameAttribute), false);

            if(attributes.Length > 0)
            {
                return (attributes[0] as EventNameAttribute).EventName;
            }

            return PascalToKebabCase(eventType.Name);
        }


        public string ResolveFault(Type eventType)
        {
            var regularEventName = Resolve(eventType);
            return string.Concat(regularEventName, ":FAULT");
        }

        private string PascalToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }

    }
}
