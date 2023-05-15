namespace ShopApp.entity
{
    public class ProductCategory
    {
        //çoka çok tablo yapısı ıcın cansn tablo hazırladık ve sekıldekı gıbı tanımldık
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}