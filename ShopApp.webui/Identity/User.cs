using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ShopApp.webui.Identity
{
    public class User:IdentityUser
    {
      public string FirtName { get; set; }
      public string LastName { get; set; }
      
    }
}