namespace Rabbiter.IntegrationTests.Common
{
    using AutoFixture;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Rabbiter.Core;
    using Rabbiter.Core.Abstractions.Config;
    using Rabbiter.Core.Abstractions.Publishers;
    using Rabbiter.Core.Config;
    using Rabbiter.Core.Events.Listeners;
    using Rabbiter.IntegrationTests.Events;
    using RabbitMQ.Client;
    using System;
    using System.Threading.Tasks;

    public class IntegrationTestFixture
        : IDisposable
    {
        private IHost _host;
        public Config TestConfiguration { get; private set; }

        public IEventPublisher Publisher { get; private set; }
        public TestEventHandler TestEventHandler { get; private set; }

        public Fixture Fixture { get; private set; }

        public IntegrationTestFixture()
        {
            InitializeAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            Fixture = new Fixture();

            var config
                = new ConfigurationBuilder()
                    .AddJsonFile("testsettings.json", false)
                    .AddEnvironmentVariables()
                    .Build();

            _host = await StartServerAsync(TestConfiguration = config.Get<Config>());
            
            foreach(var listener in _host.Services.GetServices<EventListener>())
            {
                await listener.StartListeningAsync(Fixture.Create<string>());
            }

            Publisher = _host.Services.GetRequiredService<IEventPublisher>();
            TestEventHandler = _host.Services.GetRequiredService<TestEventHandler>();
            
        }

        private static async Task<IHost> StartServerAsync(Config config)
        {
            var host = new HostBuilder()
                 .ConfigureServices((hostContext, services) =>
                 {

                     services.AddSingleton<TestEventHandler>();

                     services.RegisterRmqTransport(
                         () =>
                             RmqConfigurationBuilder
                                 .UseConnectionString(config.RabbitMqConnectionString)
                                 .Build()
                     );

                     services.RegisterEventListener(
                        eventGroupId: "Test", // queue name
                        options =>
                            options
                                .SubscribeOn<TestEvent, TestEventHandler>()
                                .SubscribeOn<TestEvent2, TestEventHandler>()
                    );


                 })
                 .Build();

            await host.StartAsync();

            return host;
        }

        protected void CleanupHost()
        {
            if (_host == null)
                return;
            _host.StopAsync().Wait();
            _host.Dispose();
            _host = null;
            RemoveTestQueue();
        }

        private void RemoveTestQueue()
        {
            var factory = new ConnectionFactory() { Uri = new Uri(TestConfiguration.RabbitMqConnectionString) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDelete("Test");
            }
        }

        public void Dispose()
        {
            CleanupHost();
        }
    }
}
