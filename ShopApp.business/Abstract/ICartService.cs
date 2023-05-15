using ShopApp.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.business.Abstract
{
    public interface ICartService
    {
         void InitializeCart(string userId);//burda ılk useer regıster oldugunda dırek cart toblosunda kendıne cart tablosunde yer alır
         Cart GetCartByUserId(string userId);//((userId uzerındne cart ekleme))

        void AddToCart(string userId,int productId,int quantity);//cart varmı yokmu kontrolu varsa ıcınde urun varmı yokmy kontroluvarsa uzerıne urun ekleme
        void DeleteFromCart(string userId,int productId);//cart ıcındekı ıstenılen urun sılme
        void ClearCart(int cartId);//CartItems tablosunu sılme
    }
}
