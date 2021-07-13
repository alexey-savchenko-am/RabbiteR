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
        public TestEventListener TestEventListener { get; private set; }

        public Fixture Fixture { get; private set; }

        public IntegrationTestFixture()
        {
            InitializeAsync().GetAwaiter().GetResult();
        }

        protected async Task InitializeAsync()
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
            TestEventListener = _host.Services.GetRequiredService<TestEventListener>();
            
        }

        private static async Task<IHost> StartServerAsync(Config config)
        {
            var host = new HostBuilder()
                 .ConfigureServices((hostContext, services) =>
                 {

                     services.AddSingleton<TestEventListener>();

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
                                .SubscribeOn<TestEvent, TestEventListener>()
                                .SubscribeOn<TestEvent2, TestEventListener>()
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

        }

        public void Dispose()
        {
            CleanupHost();
        }
    }
}
