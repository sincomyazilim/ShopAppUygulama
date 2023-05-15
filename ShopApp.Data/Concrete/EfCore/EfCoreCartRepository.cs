using Microsoft.EntityFrameworkCore;
using ShopApp.data.Concrete.EfCore;
using ShopApp.Data.Abstract;
using ShopApp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.Data.Concrete.EfCore
{
    public class EfCoreCartRepository : EfCoreGenericRepository<Cart, ShopContext>, ICartRepository
    {
        public void ClearCart(int cartId)
        {
            using (var context = new ShopContext())
            {
                var cmd = @"Delete from CartItems where CartId=@p0";
                context.Database.ExecuteSqlRaw(cmd, cartId);
            }
        }

        public void DeleteFromCart(int CartId, int productId)
        {
            using (var context=new ShopContext())
            {
                var cmd = @"Delete from CartItems where CartId=@p0 and ProductId=@p1";
                context.Database.ExecuteSqlRaw(cmd, CartId, productId);
            }
        }

        public Cart GetByUserId(string userId)
        {
            using (var context=new ShopContext())
            {
                return context.Carts
                              .Include(i => i.CartItems)
                              .ThenInclude(i => i.Product)
                              .FirstOrDefault(i => i.UserId == userId); ;
            }
                 
        }

        public override void Update(Cart entity)
        {
            using (var context = new ShopContext())
            {
                //context.Entry(entity).State = EntityState.Modified;//burda form uzerınde gucelleme yaptıgımızda hangı alanlar degıstıgını otomatık anlıyor ve ona gore degısen kısım ıcın guncelleme yapıyor
                //context.SaveChanges();
                //bunları ezıyoruzz uzerıne kod yazıyor yani

                context.Carts.Update(entity);
                context.SaveChanges();
                
            }
        }
    }
}
