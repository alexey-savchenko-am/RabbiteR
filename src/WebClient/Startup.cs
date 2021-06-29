namespace WebClient
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Rabbiter.Core;
    using Rabbiter.Core.Config;
    using WebClient.Data;
    using WebClient.EventHandlers;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Microsoft.OpenApi.Models;
    using WebClient.Event;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }



        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ShopContext>(c =>
                c.UseInMemoryDatabase("ShopDB"));

            services.AddScoped<ShopFaultOrderRequestedEventHandler>();
            services.AddScoped<PaymentsOrderRequestedEventHandler>();
            services.AddScoped<ReportingOrderRequestedEventHandler>();
            services.AddScoped<ReportingOrderAcceptedEventHandler>();

            services.RegisterRmqTransport(
                () =>
                    RmqConfigurationBuilder
                        .UseConnectionString(Configuration["RabbitMqConnectionString"])
                        .Build(),
                payloadFormat: PayloadFormats.Json
            );


            // Imitate Shop service
            services.RegisterEventListener(
                eventGroupId: "Shop", // queue name
                options =>
                    options
                        .OnFault<OrderRequestedEvent, ShopFaultOrderRequestedEventHandler>()
            );


            // Imitate Payments service
            services.RegisterEventListener(
                eventGroupId: "Payments", // queue name
                options => 
                    options
                        .SubscribeOn<OrderRequestedEvent, PaymentsOrderRequestedEventHandler>()
            );

            // Imitate Reporting service
            services.RegisterEventListener(
                eventGroupId: "Reporting", // queue name
                options =>
                    options
                        .SubscribeOn<OrderRequestedEvent, ReportingOrderRequestedEventHandler>()
                        .SubscribeOn<OrderAcceptedEvent, ReportingOrderAcceptedEventHandler>()
            );


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rabbiter.API", Version = "v1" });
                c.EnableAnnotations();
            });

            services.AddLogging();
            services.AddMvc();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rabbiter.API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=rabbiter}/{action=index}");
            });
        }
    }
}
