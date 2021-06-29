using System.Text.RegularExpressions;

namespace Rabbiter.Core.Abstractions.Config
{
    public class RmqHost
    {

        public static bool TryParse(string src, int defaultPort, out RmqHost result)
        {
            result = null;

            if (string.IsNullOrEmpty(src))
                return false;

            src = src.ToLowerInvariant();
            var match = Regex.Match(src, @"^(?<host>[^:]+)(:(?<port>\d+))?$");
            if (match.Success)
            {
                var host = match.Groups["host"].Value;
                var portString = match.Groups["port"].Value;
                var port = defaultPort;
                if (portString.Length != 0 && !int.TryParse(portString, out port))
                    return false;

                result = new RmqHost
                {
                    Host = host,
                    Port = port
                };
                return true;
            }

            return false;
        }

        public string Host { get; set; }
        public int Port { get; set; }
    }
}
