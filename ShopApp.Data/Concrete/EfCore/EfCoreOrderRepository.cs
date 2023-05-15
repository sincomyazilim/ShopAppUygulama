using Microsoft.EntityFrameworkCore;
using ShopApp.data.Abstract;
using ShopApp.data.Concrete.EfCore;
using ShopApp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.Data.Concrete.EfCore
{
    public class EfCoreOrderRepository : EfCoreGenericRepository<Order, ShopContext>, IOrderRepository
    {
        public List<Order> GetOrders(string userId)
        {
            using (var context=new ShopContext())
            {
                var orders = context.Orders
                                  .Include(i => i.OrderItems)
                                  .ThenInclude(i => i.Product)
                                  .AsQueryable();

                if (!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(i => i.UserId == userId);
                }
                return orders.ToList();
            }
        }
    }
}
