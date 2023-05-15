using ShopApp.entity;
using System.Collections.Generic;

namespace ShopApp.data.Abstract
{
    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetOrders(string userId);
    }
}