using System;

namespace Shared.Order.Contracts
{
    public interface IDeleteOrder
    {
        Guid OrderId { get; set; }
    }
}