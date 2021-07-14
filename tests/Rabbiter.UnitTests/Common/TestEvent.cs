namespace Rabbiter.UnitTests.Common
{
    using Newtonsoft.Json;
    using Rabbiter.Core.Abstractions.Events;

    class TestEvent : IEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
