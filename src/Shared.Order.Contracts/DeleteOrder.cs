using System;

namespace Shared.Order.Contracts
{
    public class DeleteOrder : IDeleteOrder
    {
        public Guid OrderId { get; set; }
    }
}