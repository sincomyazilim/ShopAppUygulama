using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.webui.Identity
{
    public static class SeedIdentity
    {
        //vıdeo 619
        public static async Task Seed(UserManager<User> userManager,RoleManager<IdentityRole> roleManager,IConfiguration configuration)//,IConfiguration configuration eklenme sebebi appsettin ıcıne kayıt edılen admın bılgılerıne ulasmak ıcndır
        {//burda appsettıntekı admın bılgılerını cekıyruz
            var username = configuration["Data:AdminUser:username"];
            var email = configuration["Data:AdminUser:email"];
            var password = configuration["Data:AdminUser:password"];
            var role = configuration["Data:AdminUser:role"];


             
            if (await userManager.FindByNameAsync(username)==null)//bu username adında kullancı yoksa
            {
                await roleManager.CreateAsync(new IdentityRole(role));// roletablosunua role eklıyoruz

                var user = new User()//user bıgılerı eklenıyor
                {
                    UserName = username,
                    Email=email,
                    FirtName="admin",
                    LastName="admin",
                    EmailConfirmed=true
                    

                };

                var result = await userManager.CreateAsync(user, password);//user tablosuna eklenıyor
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);// burdada rolle bırlıkte user eklenıyor
                }

            }
        }
    }
}
