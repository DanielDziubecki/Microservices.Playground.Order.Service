using System;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Order.Service.Integration.Tests
{
    public class HostInitialization : IDisposable
    {
        public readonly TestServer Server;
        public readonly HttpClient Client;

        public HostInitialization()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Test")
                .UseStartup<TestStartup>();
            
            Server = new TestServer(webHostBuilder);
            Client = Server.CreateClient();
        
        }

        public void Dispose()
        {
            Server?.Dispose();
            Client?.Dispose();
        }

        public class TestStartup : Startup
        {
            public TestStartup(IHostingEnvironment env) : base(env)
            {
                env.EnvironmentName = "test";
            }

            public IServiceProvider ConfigureTestServices(IServiceCollection services)
            {
                var container = new ContainerBuilder();
               //configure test infrastructure here
                return new AutofacServiceProvider(container.Build());
            }
        }
    }
}