using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopApp.business.Abstract;
using ShopApp.entity;
using ShopApp.webui.Extensions;
using ShopApp.webui.Identity;
using ShopApp.webui.Models;

namespace ShopApp.webui.Controllers {
    
    //admin kım ıze bu sayfayı görebılır
    [Authorize(Roles ="admin")]
    public class AdminController : Controller {
        
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public AdminController (IProductService productService,ICategoryService categoryService, 
                                RoleManager<IdentityRole> roleManager,UserManager<User> userManager) 
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        //************************roller bölümü**************************
        //user bölümü****************
        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);//yukardan gelen id bılgısıne göre  user bul
            if (user!=null)//bunlunduysa
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);//user aıt rollerıde bul
                var roles = _roleManager.Roles.Select(i => i.Name);//bu arada roller tablsuna gıt tum rollerın ısımlerını al

                ViewBag.Roles = roles;//bu rollerın isimlerini vıewbag pakatle lazım olacak
                return View(new UserDetailsModel()
                {  //sonra view uzerınden UserDetailsModel doldur gonder,ıd ıle gelen userları doldur

                    UserId =user.Id,
                    FirstName=user.FirtName,
                    LastName=user.LastName,
                    UserName=user.UserName,
                    Email=user.Email,
                    EmailConfirmed=user.EmailConfirmed,
                    SelectedRoles=selectedRoles

                });
            }
            return Redirect("~/admin/user/list");
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model,string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);//modelden gelen userId göre user bılgı cekme
                if (user != null)//bs degılse yenı bılgılerı eskılerle degıstır
                {
                    user.FirtName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;//maıl ile onaylanan onaysız hale getırebılırız tekrar

                    var result = await _userManager.UpdateAsync(user);//user tablosunu guncelle
                    if (result.Succeeded)
                    {//video 618 anlatılıyor
                        var userRoles = await _userManager.GetRolesAsync(user);//formla bırlıkte gelen user aıt roles gelıyor
                        selectedRoles = selectedRoles ?? new string[] { };//boş gelırse hata vermeısn dıye eklenıyor new string[]{}
                        await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());//selectedRoles.Except(userRoles).ToArray<string>() bunun alamı secılmeen ve secılen roller arasında fark varsa onu ekelr
                        await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles).ToArray<string>());
                        

                        return Redirect("/admin/user/list");
                    }


                    
                }

                return Redirect("/admin/user/list");

            }
            return View(model);
        }
        public IActionResult UserList()
        {
            var userList = _userManager.Users ;
            return View(userList);
        }

        //************ users bölümü******************

        public IActionResult RoleList(string id)
        {
            var roleList = _roleManager.Roles;
            return View(_roleManager.Roles);
        }
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users)
            {//asagıdakı yazdıgımız kodda list tanımlayıp member mı nonmember mı ona gore lıste eklenıyor
                
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
                list.Add(user);

            }

            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers=nonmembers


            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ??new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user!=null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("",error.Description);
                            }

                        }
                    }
                }

                foreach (var userId in model.IdsToDelete ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }

                        }
                    }
                }

                
            }
            return Redirect("/admin/role/" + model.RoleId);
        }
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                 if (result.Succeeded)
                 {
                    return RedirectToAction("RoleList");
                 }
                else
                {
                    foreach (var error in result.Errors)// bu kod tanımlıolan role tablosundakı olabılecek hatalar nelerdır bılmedıgımızden hepsını burda sıraladık
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            
            return View(model);
        }

        //******************roller bölümü son
        public IActionResult ProductList () {
            return View (new ProductListViewModel () {
                Products = _productService.GetAll ()
            });
        }

        [HttpGet] //yazılsada olur yazılmazsada aynı görevı yapar
        public IActionResult ProductCreate () {
            return View ();
        }

        [HttpPost]
        public IActionResult ProductCreate (ProductModel model) {
            if (ModelState.IsValid) {
                var entity = new Product () {
                    Name = model.Name,
                    Url = model.Url,
                    Price = model.Price,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl
                };
                if (_productService.Create (entity)) {

                    //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Ürün başarıyla eklendi.",
                        Message = "Ürün eklendi.",
                        AlertType = "success"

                    });

                    return RedirectToAction ("ProductList");
                }

                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Hata",
                    Message = _productService.ErrorMessage,
                    AlertType = "danger"

                });
                  //CreateMessage (_productService.ErrorMessage, "danger");
               
                
                return View (model);
            }
            //modelState.Isvalid deglse yanı hatalı form gırıs varsa 
            return View (model);

        }

        [HttpGet]
        public IActionResult ProductEdit (int? id) {
            if (id == null) {
                return NotFound ();
            }
            var entity = _productService.GetByIdWithCategories ((int) id);
            if (entity == null) {
                return NotFound ();

            }
            var model = new ProductModel () {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Url = entity.Url,
                ImageUrl = entity.ImageUrl,
                Description = entity.Description,
                Price = entity.Price,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,
                SelectedCategories = entity.ProductCategories.Select (i => i.Category).ToList ()
            };
            ViewBag.Categories = _categoryService.GetAll ();
            return View (model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit (ProductModel model, int[] categoryIds, IFormFile file) //int categorid ekleme sebebmız product tablosuna eklenınce secılı kategorılerde productcategorı tablusuna eklenacelk
        {
            if (ModelState.IsValid) {
                var entity = _productService.GetById (model.ProductId);
                if (entity == null) {
                    NotFound ();
                }
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Price = model.Price;
                entity.Url = model.Url;
                entity.ImageUrl = model.ImageUrl;

                entity.IsApproved = model.IsApproved;
                entity.IsHome = model.IsHome;

                if (file != null) {

                    var extentionUzanti = Path.GetExtension (file.FileName);
                    var randomName = string.Format ($"{Guid.NewGuid()}{extentionUzanti}");

                    entity.ImageUrl = randomName;
                    var path = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot\\img", randomName);

                    using (var stream = new FileStream (path, FileMode.Create)) {
                        await file.CopyToAsync (stream);
                    }

                }

                if (_productService.Update (entity, categoryIds)) {
                    //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Kayıt Güncellendi.",
                        Message = "Kayıt Güncellendi.",
                        AlertType = "success"

                    });
                    return RedirectToAction ("ProductList");
                }
                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Hata",
                    Message = _productService.ErrorMessage,
                    AlertType = "danger"

                });
                //CreateMessage (_productService.ErrorMessage, "danger");

            }
            ViewBag.Categories = _categoryService.GetAll ();
            return View (model);
        }

        public IActionResult DeleteProduct (int productId) {
            var entity = _productService.GetById (productId);
            if (entity != null) {
                _productService.Delete (entity);
            }
            //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
            TempData.Put("message", new AlertMessage()
            {
                Title = "Kayıt Silindi.",
                Message = $"{entity.Name} isimli ürün Silindi",
                AlertType = "danger"

            });

            //var msg = new AlertMessage () {
            //    Message = $"{entity.Name} isimli ürün Silindi",
            //    AlertType = "danger"
            //};
            //TempData["Message"] = JsonConvert.SerializeObject (msg);

            return RedirectToAction ("ProductList");

        }

        //category bölümü list edit delete yapılacak
        public IActionResult CategoryList () {
            return View (new CategoryListViewModel () {
                Categories = _categoryService.GetAll ()
            });
        }

        [HttpGet] //yazılsada olur yazılmazsada aynı görevı yapar
        public IActionResult CategoryCreate () {
            return View ();
        }

        [HttpPost]
        public IActionResult CategoryCreate (CategoryModel model) {
            if (ModelState.IsValid) {
                var entity = new Category () {
                    Name = model.Name,
                    Url = model.Url

                };
                _categoryService.Create (entity);

                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Kayıt eklendi.",
                    Message = $"{entity.Name} isimli Kategori eklendi",
                    AlertType = "success"

                });

                //var msg = new AlertMessage () {
                //    Message = $"{entity.Name} isimli Kategori eklendi",
                //    AlertType = "success"
                //};
                //TempData["Message"] = JsonConvert.SerializeObject (msg); //burda alert mesajj cıkartmak ıcın json data uretıyoruz
                return RedirectToAction ("CategoryList");
            }
            return View (model);
        }

        [HttpGet]
        public IActionResult CategoryEdit (int? id) {
            if (id == null) {
                return NotFound ();
            }
            var entity = _categoryService.GetByIdWithProducts ((int) id);
            if (entity == null) {
                return NotFound ();

            }
            var model = new CategoryModel () {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select (p => p.Product).ToList ()

            };
            return View (model);
        }

        [HttpPost]
        public IActionResult CategoryEdit (CategoryModel model) {
            if (ModelState.IsValid) {
                var entity = _categoryService.GetById (model.CategoryId);
                if (entity == null) {
                    NotFound ();
                }
                entity.Name = model.Name;
                entity.Url = model.Url;

                _categoryService.Update (entity);

                //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Kayıt Güncellendi.",
                    Message = $"{entity.Name} isimli Kategori güncellendi",
                    AlertType = "success"

                });


                //var msg = new AlertMessage () {
                //    Message = $"{entity.Name} isimli Kategori güncellendi",
                //    AlertType = "success"
                //};
                //TempData["Message"] = JsonConvert.SerializeObject (msg);


                return RedirectToAction ("CategoryList");
            }
            if (model.Products == null) {
                model.Products = new List<Product> ();
            }
            return View (model);

        }

        public IActionResult DeleteCategory (int categoryId) {
            var entity = _categoryService.GetById (categoryId);
            if (entity != null) {
                _categoryService.Delete (entity);
            }

            //bu kısım message göstermek ıcındır yazılan bır clastır 613 vıdeo detaylıdır
            TempData.Put("message", new AlertMessage()
            {
                Title = "Kayıt Silindi.",
                Message = $"{entity.Name} isimli Kategori Silindi",
                AlertType = "danger"

            });


            //var msg = new AlertMessage () {
            //    Message = $"{entity.Name} isimli Kategori Silindi",
            //    AlertType = "danger"
            //};
            //TempData["Message"] = JsonConvert.SerializeObject (msg);
            return RedirectToAction ("CategoryList");

        }

        [HttpPost]
        public IActionResult DeleteFromCategory (int productId, int categoryId) {
            _categoryService.DeleteFromCategory (productId, categoryId);
            return Redirect ("/admin/categories/" + categoryId);
        }
        //private void CreateMessage (string message, string alerttype) {
        //    var msg = new AlertMessage () {
        //        Message = message,
        //        AlertType = alerttype
        //    };
        //    TempData["Message"] = JsonConvert.SerializeObject (msg); //burda alert mesajj cıkartmak ıcın json data uretıyoruz
        //}
    }

}