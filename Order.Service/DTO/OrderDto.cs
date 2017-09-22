using System;

namespace Order.Service.DTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; }
        public string Quantity { get; set; }
    }
}