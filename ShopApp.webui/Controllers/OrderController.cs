using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.business.Abstract;
using ShopApp.webui.Identity;
using ShopApp.webui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.webui.Controllers
{
    public class OrderController : Controller
    {
        private UserManager<User> _userManager;
        private IOrderService _orderService;

        public OrderController(IOrderService orderService, UserManager<User> userManager)
        {
            this._orderService = orderService;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult GetOrders()
        {
           var userId = _userManager.GetUserId(User);
           var orders= _orderService.GetOrders(userId);

           var orderlistModel = new List<OrderListModel>();

            OrderListModel orderModel;
            foreach (var order in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.Phone = order.Phone;
                orderModel.FirstName = order.FirstName;
                orderModel.Email = order.Email;
                orderModel.Address = order.Address;
                orderModel.City = order.City;
                orderModel.Note = order.Note;
                orderModel.OrderState = order.OrderState;
                orderModel.PaymentType = order.PaymentType;

                orderModel.OrderItems = order.OrderItems.Select(i => new OrderItemModel()
                {
                    OrderItemId=i.Id,
                    Name=i.Product.Name,
                    Price=i.Price,
                    Quantity=i.Quantity,
                    ImageUrl=i.Product.ImageUrl

                }).ToList();

                orderlistModel.Add(orderModel);

            }



            return View("Orders", orderlistModel);
        }
    }
}
