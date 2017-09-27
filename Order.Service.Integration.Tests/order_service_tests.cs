using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Order.Service.DTO;
using Xunit;

namespace Order.Service.Integration.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class order_service_tests : IClassFixture<HostInitialization>
    {
        private readonly HostInitialization host;

        public order_service_tests(HostInitialization host)
        {
            this.host = host;
        }

        [Fact]
        public async void operation_id_should_be_taken_from_request_headers()
        {
            var content =  new StringContent(
                    JsonConvert.SerializeObject(new OrderDto {Id = Guid.NewGuid(), ProductId = "3", Quantity = "2"}), Encoding.UTF8, "application/json");
            content.Headers.Add("operationid", Guid.NewGuid().ToString());
            var response = await host.Client.PostAsync("api/order", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void request_without_operationid_header_should_return_404_code()
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(new OrderDto { Id = Guid.NewGuid(), ProductId = "3", Quantity = "2" }), Encoding.UTF8, "application/json");
            var response = await host.Client.PostAsync("api/order", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    
}