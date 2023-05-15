using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopApp.business.Abstract;
using ShopApp.webui.EmailServices;
using ShopApp.webui.Extensions;
using ShopApp.webui.Identity;
using ShopApp.webui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.webui.Controllers
{
    [AutoValidateAntiforgeryToken]
    //[Authorize]
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;// kullancı bılgılerı eklemek gırıs yapmak ıcın kulanılıyor
        private SignInManager<User> _signInManager;//cookı yonetmek ıcın kullanılacak
        private IEmailSender _emailSender;//mail atma içındı
        private ICartService _cartService;// cart eklemek ıcın


        public AccountController(UserManager<User> userManager ,SignInManager<User> signInManager, 
                                IEmailSender emailSender, ICartService cartService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailSender = emailSender;
            this._cartService = cartService;
        }
        //[AllowAnonymous]
        public IActionResult Login(string ReturnUrl = null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            }); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user==null)
            {
                ModelState.AddModelError("", "Bu Kullanıcı veya Email Kayıtlı Değil");
                return View(model);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen hesabınıza gelen link ile Üyeliğinizi onaylayınız");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true,false);
            if (result.Succeeded)
            { 
               
                return Redirect(model.ReturnUrl??"~/");//?? null olup olmadıgına bakıyor nuul degılse model.ReturnUlr ye gıder null ise ~/  anasayfa demek ona gıder
            }
            ModelState.AddModelError("","Kullancı adı veya Şifre yanlış");
            return View(model);
        }
       // [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirtName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // generate token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });

                //email
               await _emailSender.SendEmailAsync(model.Email, "Hesabınızı onaylayınız.", $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:44326{url}'>tıklayınız.</a>");
               return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Bilinmeyen hata oldu lütfen tekrar deneyiniz.");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();//cookı sılıp cıkıs yapıyor
                                                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
            TempData.Put("message", new AlertMessage()
            {
                Title = "Oturum Kapandı.",
                Message = "Hesabınız Güvenli bir şekilde kapatıldı.",
                AlertType = "success"

            });
            return Redirect("~/");
        }


        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if (userId==null||token==null)
            {
                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message" ,new AlertMessage() { 
                    Title= "Geçersiz Token",
                    Message="Geçersiz Token",
                    AlertType="danger"

                });
               
              
                    return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user!= null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //Her eklenen user ıcın otomatok maıl ve otomatık cart tanımlanac
                    //Cart ekleme****************
                    _cartService.InitializeCart(userId);//kart tanımlandı bu user'a

                    //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Hesabınız Onaylandı.",
                        Message = "Hesabınız Onaylandı.",
                        AlertType = "success"

                    });
                    
                    return View();
                }
            }
                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Hesabınız Onaylanmadı.",
                    Message = "Hesabınız Onaylandı tekrar kontrol edınız.",
                    AlertType = "warning"

                });
            
            return View();
            
            
        }

       // [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return View();

            }
            var user = await _userManager.FindByEmailAsync(Email);
            if (user==null)
            {
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = code
            });

            //email
            await _emailSender.SendEmailAsync(Email, "Şifre Sıfırlama", $"Lütfen Şifreninizi yenilemek için linke <a href='https://localhost:44326{url}'>tıklayınız.</a>");
            return RedirectToAction("Login", "Account");
          
        }


        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId==null||token==null)
            {   //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Kullanıcı veya kod hatalıdır",
                    Message = "kullancı veya kod hatalıdır.",
                    AlertType = "danger"

                });
               
                return RedirectToAction("Login","Account");

            }
            var model = new ResetPasswordModel
            {
                Token = token
            };
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Bu email kayıtlı değildir.",
                    Message = " üyelğiniz yok yada yanlış yazıldı",
                    AlertType = "danger"

                });

                
                return RedirectToAction("Login", "Account");

            }
            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login","Account");
            }
             
            return View(model);
        }


        public IActionResult AccessDenied()
        {
            return View();
        }






        //private void CreateMessage(string message, string alerttype)
        //{// bu medtotla mesaj yazdırıyorudk bunun ıcın clas elklıdn ve ordan agonderılecegı ıcın burayı pasıf ettık
        //    var msg = new AlertMessage()
        //    {
        //        Message = message,
        //        AlertType = alerttype
        //    };
        //    TempData["Message"] = JsonConvert.SerializeObject(msg); //burda alert mesajj cıkartmak ıcın json data uretıyoruz
        //}
    }
}
