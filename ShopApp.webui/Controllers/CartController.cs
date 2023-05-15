using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.business.Abstract;
using ShopApp.webui.Extensions;
using ShopApp.webui.Identity;
using ShopApp.webui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Request;
using Iyzipay.Model;
using ShopApp.entity;
using Newtonsoft.Json;

namespace ShopApp.webui.Controllers
{   [Authorize]
    public class CartController : Controller
    { //depesing injectıon*****************************
        private ICartService _cartService;
        private UserManager<User> _userManager;
        private IOrderService _orderService;

        public CartController(ICartService cartService, UserManager<User> userManager, IOrderService orderService)
        {
            this._cartService = cartService;
            this._userManager = userManager;
            this._orderService = orderService;
        }

        //depesing injectıon**************************
        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));//burda grıs yapan user uzerınden onun cartına ulasıyoruz

            if (cart != null)
            {


                return View(new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i => new CartItemModel()
                    {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity

                    }).ToList()

                });
            }
            else
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Bilgilendirme",
                    Message = "Onayınız Admin Tarafınfan Onaylandığı için Kart tanımı yapılmamış",
                    AlertType = "danger"

                });
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public IActionResult AddToCart(int productId,int quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.AddToCart(userId, productId, quantity);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId, productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));//burda grıs yapan user uzerınden onun cartına ulasıyoruz

            var orderModel = new OrderModel();
            orderModel.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (double)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity

                }).ToList()

            };

                return View(orderModel);
           
        }

        [HttpPost]
        public IActionResult Checkout(OrderModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);//userID tespıt edıyoruz   
                var cart = _cartService.GetCartByUserId(userId);//cart ııcndekıurunlerı getırıyoruz

                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i => new CartItemModel()
                    {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity

                    }).ToList()
                };
                var payment = PaymentProcess(model);//model
                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);//altakı metodu cagır
                    ClearCart(model.CartModel.CartId);//alttekı metodu cagır

                    return View("Success");
                }
                else
                {
                    var msg = new AlertMessage()
                    {
                        Message = $"{payment.ErrorCode}",
                        AlertType = "danger"
                    };
                    TempData["Message"] = JsonConvert.SerializeObject(msg);
                
                }
            }
            return View(model);

        }

        private void ClearCart(int cartId)
        {
            _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            var order = new Order();

            order.OrderNumber = new Random().Next(111111, 999999).ToString();
            order.OrderState = EnumOrderState.completed;
            order.PaymentType = EnumPaymentType.CreditCart;

            order.PaymentId = payment.PaymentId;//payment servısı uzerınden alıyoruz bu kısmını
            order.ConversationId = payment.ConversationId;//pamnet servısı uzerınde alınıyor

            order.OrderDate = new DateTime();

            order.UserId = userId;//userıd bölumu
            order.FirstName = model.FirstName;//model uzerınden dolduruluyor
            order.LastName = model.LastName;
            order.Address = model.Address;
            order.Phone = model.Phone;
            order.Email = model.Email;
            order.City = model.City;
            order.Note = model.Note;

            order.OrderItems = new List<entity.OrderItem>();
            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new ShopApp.entity.OrderItem()
                {
                    Price=item.Price,
                    ProductId=item.ProductId,
                    Quantity=item.Quantity
                };


              
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);//veritaba kayıt edılecek

        }

        private Payment PaymentProcess(OrderModel model)
        {//API BAGLANTILARI
            Options options = new Options();
            options.ApiKey = "sandbox-MaG6UWLHiFarssfTz2fRtUlIlHzd8QgI";//video 629 
            options.SecretKey = "sandbox-KycCr2mths6aN6RbTlr9ROqpVozfLiKj";//video 629
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            //URUN BILGI VE TOTAL FİYAT+KARGOLAMA FIYATI
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(111111111,999999999).ToString();//video 630
            request.Price = model.CartModel.TotalPrice().ToString();//video 630
            request.PaidPrice = model.CartModel.TotalPrice().ToString();//video 630 burta gargo parası eklenebılır
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();


            //KREDI KARTI KULLANILACAKSA, KULANILACAK KART BILGILERI
            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;//video 630
            paymentCard.CardNumber = model.CardNumber;//video 630
            paymentCard.ExpireMonth = model.ExpirationMonth;//video 630
            paymentCard.ExpireYear = model.ExpirationYear;//video 630
            paymentCard.Cvc = model.Cvc;//video 630
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";

            //ALICI BILGILERI BURDA GIRILIYOR
            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            
            request.Buyer = buyer;

            //ALICI ADRESI
            Address shippingAddress = new Address();
            shippingAddress.ContactName = model.FirstName+" "+model.LastName;
            shippingAddress.City = model.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = model.Address;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var item in model.CartModel.CartItems)
            {
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Telefon";
                basketItem.Price = item.Price.ToString();
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItems.Add(basketItem);

            }
            request.BasketItems = basketItems;
            //BasketItem firstBasketItem = new BasketItem();
            //firstBasketItem.Id = "BI101";
            //firstBasketItem.Name = "Binocular";
            //firstBasketItem.Category1 = "Collectibles";
            //firstBasketItem.Category2 = "Accessories";
            //firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            //firstBasketItem.Price = "0.3";
            //basketItems.Add(firstBasketItem);

                 

           

           return Payment.Create(request, options);


        }

    }
}
