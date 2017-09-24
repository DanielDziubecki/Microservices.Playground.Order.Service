using System;

namespace Shared.Order.Contracts
{
    public interface IOrderSubmitted
    {
         Guid OrderId { get; set; }
    }
}