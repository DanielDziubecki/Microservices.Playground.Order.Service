using System;

namespace Shared.Order.Contracts
{
    public class OrderSubmitted : IOrderSubmitted
    {
        public Guid OrderId { get; set; }
    }
}