using System;
using System.Collections.Generic;
using System.Linq;
using Order.Service.DTO;

namespace Order.Service.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly List<OrderDto> orders = new List<OrderDto>
        {
            new OrderDto() {ProductId = "1", Id =Guid.NewGuid(), Quantity = "1"},
            new OrderDto() {ProductId = "2", Id =Guid.NewGuid(), Quantity = "2"},
            new OrderDto() {ProductId = "3", Id =Guid.NewGuid(), Quantity = "5"},
            new OrderDto() {ProductId = "6", Id = Guid.NewGuid(), Quantity = "8"},
        };

        public void AddOrder(OrderDto order)
        {
            orders.Add(order);
        }

        public OrderDto GetOrder(Guid id)
        {
            return orders.SingleOrDefault(x => x.Id == id);
        }
    }
}