using ShopApp.data.Abstract;
using ShopApp.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Data.Abstract
{
    public interface ICartRepository:IRepository<Cart>
    {
        //cart özel işlemler yapılabılır vıdeo 620
        Cart GetByUserId(string userId);
        void DeleteFromCart(int CartId, int productId);
        void ClearCart(int cartId);
    }
}
