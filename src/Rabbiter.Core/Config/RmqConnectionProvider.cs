namespace Rabbiter.Core.Config
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Rabbiter.Core.Abstractions.Config;
    using RabbitMQ.Client;

    public class RmqConnectionProvider
        : IConnectionProvider, IDisposable
    {
 

        private readonly ILogger<RmqConnectionProvider> _logger;
        private IConnection _connection;

        public RmqConnectionProvider()
        {
      
        }

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public IConnection Connect(RmqConnectionConfiguration connectionModel)
        {

            var endpoints = GetEndpoints(connectionModel.Hosts, false);

            var connectionFactory = new ConnectionFactory
            {
                UserName = connectionModel.UserName,
                Password = connectionModel.Password,
                VirtualHost = connectionModel.VirtualHost,
                AutomaticRecoveryEnabled = connectionModel.AutomaticRecoveryEnabled ?? true,
                EndpointResolverFactory = x => new DefaultEndpointResolver(endpoints)
            };


            var connection = connectionFactory.CreateConnection();

            return connection;
        }

        private IEnumerable<AmqpTcpEndpoint> GetEndpoints(IList<RmqHost> hosts, bool ssl)
        {
            foreach (var iHost in hosts)
            {
                var endpoint = new AmqpTcpEndpoint(iHost.Host, iHost.Port);

                if(ssl) endpoint.Ssl = new SslOption { Enabled = true };

                yield return endpoint;
            }
        }


        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
