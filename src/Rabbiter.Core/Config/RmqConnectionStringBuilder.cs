namespace Rabbiter.Core.Config
{
    using Rabbiter.Core.Abstractions.Config;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class RmqConnectionStringBuilder
    {
        private readonly string _connectionString;
        private const string _defaultVirtualHost = "/";
        private const int _defaultAmqpPort = -1;
        private const int _defaultAmqpsPort = 5671;

        private RmqConnectionStringBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static RmqConnectionStringBuilder UseConnectionString(string connectionString)
        {
            return new RmqConnectionStringBuilder(connectionString);
        }

        public RmqConnectionConfiguration Build()
        {
            return ProcessConnectionString(_connectionString);
        }

        // "amqp://guest:guest@localhost:15672/localhost"
        private static RmqConnectionConfiguration ProcessConnectionString(string connectionString)
        {
            const string serverPattern = @"(?<host>((\[[^]]+?\]|[^:@,/?#]+)(:\d+)?))";
            const string serversPattern = serverPattern + @"(," + serverPattern + ")*";
            const string virtualHostPattern = @"(?<virtualhost>[^/?]+)";
            const string optionPattern = @"(?<option>[^&;]+=[^&;]+)";
            const string optionsPattern = @"(\?" + optionPattern + @"((&|;)" + optionPattern + ")*)?";
            const string pattern =
                @"^(?<scheme>amqp|amqps)://" +
                @"((?<username>[^:@]+)(:(?<password>[^:@]*))?@)?" +
                serversPattern + @"(/" + virtualHostPattern + ")?/?" + optionsPattern + "$";

            var match = Regex.Match(connectionString, pattern);

            if (!match.Success)
            {
                throw new ArgumentException(string.Format("The connection string '{0}' is not valid.", connectionString));
            }

            var scheme 
                = match.Groups["scheme"].Value.ToLowerInvariant() == "amqps"
                   ? Scheme.Amqps : Scheme.Amqp;
            
            var virtualHost = Uri.UnescapeDataString(match.Groups["virtualhost"].Value);


            return new RmqConnectionConfiguration
            {
                RmqScheme = scheme,
                UserName = Uri.UnescapeDataString(match.Groups["username"].Value),
                Password = Uri.UnescapeDataString(match.Groups["password"].Value),
                Hosts = GetHostList(match, scheme),
                VirtualHost = string.IsNullOrWhiteSpace(virtualHost)
                        ? _defaultVirtualHost : virtualHost
            };
        }


        private static IList<RmqHost> GetHostList(Match match, Scheme scheme)
        {
            int defaultPort = _defaultAmqpPort;
            if (scheme == Scheme.Amqps)
            {
                defaultPort = _defaultAmqpsPort;
            }

            var hosts = new List<RmqHost>();
            foreach (Capture hostCapture in match.Groups["host"].Captures)
            {
                if (RmqHost.TryParse(hostCapture.Value, defaultPort, out var host))
                    hosts.Add(host);
                else
                    throw new ArgumentException(string.Format("Host '{0}' is not valid.", hostCapture.Value));
            }

            return hosts;
        }




    }
}
