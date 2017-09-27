using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.Service.Common;
using Order.Service.DTO;
using Order.Service.Repository;
using Shared.Order.Contracts;

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
        public async Task<IActionResult> Post([FromBody]OrderDto dto, [FromHeader(Name = "operationid")] string operationId)
        {
            if (!Guid.TryParse(operationId, out Guid operation))
                return BadRequest("Operation id should be Guid type.");

            orderRepository.AddOrder(dto);

            await busControl.Publish<IOrderCreated>(new
            {
                OrderId = dto.Id
            }, context =>
            {
                context.Headers.Set(LogConstansts.Common.OperationId, operation.ToString());
                context.Headers.Set(LogConstansts.QueueMessageHeaderNames.Publisher, Request.Path.Value);
            });

            return Ok(dto);
        }
    }
}