using System.Collections.Generic;

namespace Rabbiter.Core.Abstractions.Config
{
    public class RmqConnectionConfiguration
    {
        public Scheme RmqScheme { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public IList<RmqHost> Hosts { get; set; }

        public bool? AutomaticRecoveryEnabled { get; set; }
    }
}
