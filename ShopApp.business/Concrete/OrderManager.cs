using ShopApp.business.Abstract;
using ShopApp.data.Abstract;
using ShopApp.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.business.Concrete
{
    public class OrderManager : IOrderService
    {
        private IOrderRepository _orderRepository;

        public OrderManager(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }
        public void Create(Order entity)
        {
            _orderRepository.Create(entity);
        }

        public List<Order> GetOrders(string userId)
        {
            return _orderRepository.GetOrders(userId);
        }
    }
}
