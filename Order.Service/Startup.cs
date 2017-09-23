using System;
using System.Data.SqlClient;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Automatonymous;
using Logging.Core.Autofac;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Saga;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Order.Saga;
using Order.Service.Repository;

namespace Order.Service
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<OrderRepository>().As<IOrderRepository>();

            builder.Register(context =>
                {
                    var orderSaga = new OrderProcessSaga();

                    var dbContextOptionsBuilder =
                        new DbContextOptionsBuilder<SagaDbContext<OrderState, OrderSagaMap>>();
                    dbContextOptionsBuilder.UseSqlServer(
                        new SqlConnection(
                            @"Server=DESKTOP-J45OU76\SQLEXPRESS;Database=Orders;Integrated Security=True"),
                        optionsBuilder => { });

                    var busControl = Bus.Factory.CreateUsingRabbitMq(rabbitMqConfig =>
                    {
                        var host = rabbitMqConfig.Host(new Uri("rabbitmq://localhost/#/queues/%2F/order_state"), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        rabbitMqConfig.ReceiveEndpoint(host, "order_state",
                            e =>
                            {
                                e.StateMachineSaga(orderSaga,
                                    new EntityFrameworkSagaRepository<OrderState>(
                                        () => new SagaDbContext<OrderState, OrderSagaMap>(dbContextOptionsBuilder
                                            .Options)));
                            });

                        rabbitMqConfig.ConfigureJsonSerializer(settings =>
                        {
                            settings.NullValueHandling = NullValueHandling.Ignore;
                            return settings;
                        });
                    });
                    return busControl;
                })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();

            builder.Populate(services);

            var applicationContainer = builder.Build();

            var bus = applicationContainer.Resolve<IBusControl>();
            var recieveObs = applicationContainer.Resolve<IReceiveObserver>();
            var publishObs = applicationContainer.Resolve<IPublishObserver>();

           // bus.ConnectReceiveObserver(recieveObs);
            //bus.ConnectPublishObserver(publishObs);
            bus.Start();
            return new AutofacServiceProvider(applicationContainer);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}