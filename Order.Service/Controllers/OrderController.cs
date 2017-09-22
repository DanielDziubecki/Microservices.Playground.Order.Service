using System;
using System.Threading.Tasks;
using Logging.Model;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.Contracts.Events;
using Order.Service.DTO;
using Order.Service.Repository;

namespace Order.Service.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly IBusControl busControl;

        public OrderController(IOrderRepository orderRepository, IBusControl busControl)
        {
            this.orderRepository = orderRepository;
            this.busControl = busControl;
        }

        [Route("api/order/{id}")]
        public IActionResult Get(string id)
        {
            return Ok(id);
        }

        [Route("api/order")]
        [HttpPost]
        public async Task<IActionResult> Post(OrderDto dto)
        {
            orderRepository.AddOrder(dto);

            var value = Request.Headers[LogConstansts.Common.OperationId];
            var operationId = Guid.Parse(value).ToString();
            try
            {
                await busControl.Publish<IOrderCreated>(new
                {
                    OrderId = dto.Id
                }, context =>
                {
                    context.Headers.Set(LogConstansts.Common.OperationId,
                        operationId);
                    context.Headers.Set(LogConstansts.QueueMessageHeaderNames.Publisher, Request.Path.Value);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Ok(dto);
        }
    }
}