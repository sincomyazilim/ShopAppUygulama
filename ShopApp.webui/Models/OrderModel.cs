using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.webui.Models
{
    public class OrderModel
    {//user bılgılerını alma bölumu adres vb
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }

        //kredı kartı bılgılerı alma bölumu
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear{ get; set; }
        public string Cvc { get; set; }

        //alınan urunlerın lıstesı ve fıyat lıstesı cart yanı
        public CartModel CartModel { get; set; }
    }
}
