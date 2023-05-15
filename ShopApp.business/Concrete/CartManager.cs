using ShopApp.business.Abstract;
using ShopApp.Data.Abstract;
using ShopApp.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.business.Concrete
{
    public class CartManager : ICartService
    { // depensinh ınjection yapılıyor***************
        private ICartRepository _cartRepository;

        public CartManager(ICartRepository cartRepositor)
        {
            this._cartRepository = cartRepositor;
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);//userıd uzerınden  cart varmı yokmu bakılıyor
            if (cart!=null)//kart varsa
            {
                //eklenmek istenilen ürün seppette varmı (güncelleme )
                //eklenmek istenilen ürün sepette yok ama yeni kayıt oluştur(kayıt ekleme)
               
                var index = cart.CartItems.FindIndex(i=>i.ProductId==productId);//karta ürün varmı
                if (index<0)//o ürün yok,yeni  üründür eklenebılır 
                {
                    cart.CartItems.Add(new CartItem()
                    {
                        ProductId=productId,
                        Quantity=quantity,
                        CartId=cart.Id


                    });
                }
                else//ürün varsada aynı ürünü ekler mikarınıartırız
                {
                    cart.CartItems[index].Quantity = cart.CartItems[index].Quantity+ quantity;
                }



                _cartRepository.Update(cart);//cart update edıyoruz



            }

        }

        public void ClearCart(int CartId)
        {
            _cartRepository.ClearCart(CartId);
        }

        public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);
            if (cart!=null)
            {
                _cartRepository.DeleteFromCart(cart.Id, productId);
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            return _cartRepository.GetByUserId(userId);
        }



        // depensinh ınjection yapılıyor ********************
        public void InitializeCart(string userId)
        {
            _cartRepository.Create(new Cart() 
            {
              UserId=userId
            });
        }
    }
}
