using System;
using Order.Service.DTO;

namespace Order.Service.Repository
{
    public interface IOrderRepository
    {
        void AddOrder(OrderDto order);
        OrderDto GetOrder(Guid id);
    }
}