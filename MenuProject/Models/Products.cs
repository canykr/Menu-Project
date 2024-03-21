using System.ComponentModel.DataAnnotations;

namespace MenuProject.Models.Entities
{
    public class Products : BaseClass
    {
        [Required(ErrorMessage = "Ürün Adı Boş Geçilemez.")]
        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        public string? ProductDetails { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string CategoryName { get; set; }
    }
}
