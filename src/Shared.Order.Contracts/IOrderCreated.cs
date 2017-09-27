using System;

namespace Shared.Order.Contracts
{
    public interface IOrderCreated
    {
         Guid OrderId { get; set; }
    }
}