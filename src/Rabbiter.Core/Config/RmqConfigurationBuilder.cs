namespace Rabbiter.Core.Config
{
    using Rabbiter.Core.Abstractions.Config;
    using System.Collections.Generic;

    public class RmqConfigurationBuilder
    {
        private RmqConnectionConfiguration _configuration;


        private RmqConfigurationBuilder() {}



        public static RmqConfigurationBuilder UseScheme(Scheme scheme, bool recoveryEnabled = true)
        {
            var builder = new RmqConfigurationBuilder();
            builder._configuration = new RmqConnectionConfiguration 
            { 
                RmqScheme = scheme,
                AutomaticRecoveryEnabled = recoveryEnabled,
                Hosts = new List<RmqHost>()
            };

            return builder;
        }

        public RmqConfigurationBuilder WithHostAndPort(string host, int port)
        {
            _configuration.Hosts.Add(new RmqHost
            {
                Host = host,
                Port = port
            });

            return this;
        }


        public RmqConfigurationBuilder WithCredentials(string username, string password)
        {
            _configuration.UserName = username;
            _configuration.Password = password;

            return this;
        }


        public RmqConfigurationBuilder WithVirtualHost(string virtualHost)
        {
            _configuration.VirtualHost = virtualHost;
            return this;
        }


        /// <summary>
        /// Create <see cref="RmqConnectionConfiguration"/> using connection string
        /// </summary>
        /// <param name="connectionString">format amqp://user:pass@hostName:port/vhost</param>
        /// <returns></returns>
        public static RmqConnectionStringBuilder UseConnectionString(string connectionString)
            => RmqConnectionStringBuilder.UseConnectionString(connectionString);
        

        public RmqConnectionConfiguration Build()
        {
            return _configuration;
        }
    }
}
