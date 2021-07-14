namespace Rabbiter.Core.Messaging
{
    using Rabbiter.Core.Abstractions;
    using Rabbiter.Core.Abstractions.Events;
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonBasedPayloadTransformator
        : IPayloadTransformStrategy
    {
        private readonly JsonSerializerSettings _settings;
        public JsonBasedPayloadTransformator()
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }
       
        public TEvent ToEvent<TEvent>(string payload)
            where TEvent : IEvent
        {
           return JsonConvert.DeserializeObject<TEvent>(payload, _settings);
        }

        public IEvent ToEvent(string payload, Type eventType) 
        {
            return (IEvent)JsonConvert.DeserializeObject(payload, eventType, _settings);
        }

        public string ToMessage<TEvent>(TEvent @event) where TEvent : IEvent
        {
            return JsonConvert.SerializeObject(@event, _settings);
        }
    }
}
