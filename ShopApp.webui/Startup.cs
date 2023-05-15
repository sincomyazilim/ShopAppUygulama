using System.ComponentModel;
using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ShopApp.business.Abstract;
using ShopApp.business.Concrete;
using ShopApp.data.Abstract;
using ShopApp.data.Concrete.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ShopApp.webui.Identity;
using ShopApp.webui.EmailServices;
using Microsoft.Extensions.Configuration;
using ShopApp.Data.Abstract;
using ShopApp.Data.Concrete.EfCore;

namespace ShopApp.webui
{
    public class Startup
    {
        private IConfiguration _configuraiton;
        public Startup(IConfiguration configurition)//bu sayfaya dısardan parametre almak ıcın kullanıyoruz ornek maıl atma
        {
            this._configuraiton = configurition;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {   
                //Idenetity ıcın aspplicationcontext tanımlıyorz ve verıtaı dızayn edıyoruz
                services.AddDbContext<ApplicationContext>(options=>options.UseSqlite("Data Source=shopDb"));
                services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
           

               services.Configure<IdentityOptions>(options=>{
               //password bölumu
                options.Password.RequireDigit=true;
                options.Password.RequireLowercase=true;
                options.Password.RequireUppercase=true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric=true;


                //Lockout bölümü
                options.Lockout.MaxFailedAccessAttempts=5;
                options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers=true;

                // options.User.AllowedUserNameCharecters="";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail= true;
                options.SignIn.RequireConfirmedPhoneNumber=false;
                
                });
                 services.ConfigureApplicationCookie(options=>{
                 options.LoginPath="/account/login";
                 options.LogoutPath="/account/logout";
                 options.AccessDeniedPath="/account/accessdenied";
                 options.SlidingExpiration=true;//bu sure verıyor bıze ve ıslem yapmsak cıkıs yapıyor.ıslem yaparsak erılen sure sıfırlanır bastan baslar
                 options.ExpireTimeSpan=TimeSpan.FromMinutes(60);//yukardakı sure ıle ıstenıldgınde oynanablır
                 
                 options.Cookie=new CookieBuilder
                 {
                     HttpOnly=true,
                     Name=".ShopAppUygulama.Security.Cookie",
                     SameSite=SameSiteMode.Strict

                 };
            });

            
           

            //entity bölümü yanı sınıflar tablolar bolumu
            services.AddScoped<IProductRepository,EfCoreProductRepository>();//Product  entitiy ıcın dependency injection yapıyoruz IProductRepository bu ıstenıldıgınde buna yönlendır EfCoreProductRepository
            services.AddScoped<ICategoryRepository,EfCoreCategoryRepository>();//category entity
            services.AddScoped<ICartRepository, EfCoreCartRepository>();//cart entitiy
            services.AddScoped<IOrderRepository, EfCoreOrderRepository>();//Order entity
            
            //manager bölumu
            services.AddScoped<IProductService,ProductManager>();
            services.AddScoped<ICategoryService,CategoryManager>();
            services.AddScoped<ICartService, CartManager>();
            services.AddScoped<IOrderService, OrderManager>();

            //Email gönderme  bölümü
            services.AddScoped<IEmailSender, SmtpEmailSender>(i=>
                new SmtpEmailSender(
                    //yukarda structer metot ekledık onun uzerınden appsettings.json dosyasına ulasp bılgılerımızı alıyoruz
                    _configuraiton["EmailSender:Host"],
                    _configuraiton.GetValue<int>("EmailSender:Port"),
                    _configuraiton.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuraiton["EmailSender:UserName"],
                    _configuraiton["EmailSender:Password"]

                    ));


            services.AddControllersWithViews();//mvc controller ve view kullanabılırız artık bu tanımdan sonra
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)//(vıdeo 619)seed Identıtıy eklemek ıcın , UserManager<User> userManager, RoleManager<IdentityRole> roleManager
        {
             app.UseStaticFiles();//wwwroot klasörunu dısarıya acıyoruz
             //burda node_modules klasörünü acıruz dısarıya**
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "node_modules")),
                    RequestPath = "/node_modules"
                });
            //***********************************************
            if (env.IsDevelopment())
            {   
                SeedDatabase.Seed();//burda verıtabanımıza verı eklenecek
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();//Identıtyuser ıcın kullanıyoruz verıtabı eklenıyor bu kodla
            app.UseRouting();
            
            app.UseAuthorization();//ızın gırışlerı  ıçın

            app.UseEndpoints(endpoints =>
            {

                //orders bölümü
                endpoints.MapControllerRoute(//edit
                   name: "orders",
                   pattern: "orders",
                   defaults: new { controller = "Order", action = "GetOrders" }
               );
                //checkout sepet bölümü
                endpoints.MapControllerRoute(//edit
                   name: "checkout",
                   pattern: "checkout",
                   defaults: new { controller = "Cart", action = "checkout" }
               );
                //****cart bölümü*********

                endpoints.MapControllerRoute(//edit
                    name: "cart",
                    pattern: "cart",
                    defaults: new { controller = "Cart", action = "Index" }
                );

                //user bölümü***********
                endpoints.MapControllerRoute(//edit
                    name: "adminuseredit",
                    pattern: "admin/user/{id?}",
                    defaults: new { controller = "Admin", action = "UserEdit" }
                );


                endpoints.MapControllerRoute(//userlisteleme
                   name: "adminuserlist",
                   pattern: "admin/user/list",
                   defaults: new { controller = "Admin", action = "UserList" }
                 );
                //roles roller ekleme bölümü
                endpoints.MapControllerRoute(//role listeleme
                    name: "adminrolelist",
                    pattern: "admin/role/list",
                    defaults: new { controller = "Admin", action = "RoleList" }
                  );

                endpoints.MapControllerRoute(//edit
                    name: "adminroleedit",
                    pattern: "admin/role/{id?}",
                    defaults: new { controller = "Admin", action = "RoleEdit" }
                );
                endpoints.MapControllerRoute(//ekleme
                    name: "adminrolecreate",
                    pattern: "admin/role/create",
                    defaults: new { controller = "Admin", action = "RoleCreate" }
                );
                //admin bölümü
                //product**************
                endpoints.MapControllerRoute(
                    name:"adminproductlist",
                    pattern:"admin/products",
                    defaults:new {controller="Admin",action="ProductList"}
                );

                 endpoints.MapControllerRoute(
                    name:"adminproductedit",
                    pattern:"admin/products/{id?}",
                    defaults:new {controller="Admin",action="ProductEdit"}
                );
                 endpoints.MapControllerRoute(
                    name:"adminproductcraete",
                    pattern:"admin/products/craete",
                    defaults:new {controller="Admin",action="ProductCreate"}
                );

                
                //Category---------------------
                endpoints.MapControllerRoute(
                    name:"admincategorieslist",
                    pattern:"admin/categories",
                    defaults:new {controller="Admin",action="CategoryList"}
                );
                 endpoints.MapControllerRoute(
                    name:"admincategoryedit",
                    pattern:"admin/categories/{id?}",
                    defaults:new {controller="Admin",action="CategoryEdit"}
                );
                 
                 endpoints.MapControllerRoute(
                    name:"admincategorycraete",
                    pattern:"admin/categories/craete",
                    defaults:new {controller="Admin",action="CategoryCreate"}
                );
               
                //arama bölümü*************************

                endpoints.MapControllerRoute(
                    name:"search",
                    pattern:"search",
                    defaults:new {controller="Shop",action="search"}
                );
                //shopp bölümü--ürün detay
                endpoints.MapControllerRoute(
                    name:"productdetails",
                    pattern:"{url}",
                    defaults:new {controller="Shop",action="details"}
                );
                endpoints.MapControllerRoute(
                    name:"products",
                    pattern:"products/{category?}",
                    defaults:new {controller="Shop",action="list"}
                );
                //ana route
                  endpoints.MapControllerRoute(
                    name:"default",
                    pattern:"{controller=Home}/{action=Index}/{id?}" 
                
                );
            });

            //Idenıtytablousu ıcın kullancı eklenıyor seed data için yukardada UserManager<User> userManager, RoleManager<IdentityRole> roleManagerbunlar eklendı
            SeedIdentity.Seed(userManager,roleManager,configuration).Wait();// video 619
        }
    }
}
