using Microsoft.EntityFrameworkCore;
using ShopApp.entity;

namespace ShopApp.data.Concrete.EfCore
{
    public class ShopContext:DbContext
    {
        public DbSet<Product> Products{get;set;}
        public DbSet<Category> Categories{get;set;}
    
        public DbSet<Cart> Carts{get;set;}
        public DbSet<CartItem> CartItems{get;set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems{get;set; }
       
       


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                optionsBuilder.UseSqlite("Data Source=shopDb");
        }
        //fluenapilerımızı burda tanımlıyoruz yanı kıstlamalarımızı
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {// burda category tablosu ıle product tablusunun many to many işlemı yapılıyor yanı ıkıncıkey tanımlanıyor bırbırıne 
                modelBuilder.Entity<ProductCategory>()
                            .HasKey(c=>new {c.CategoryId,c.ProductId});
        }

    }
}