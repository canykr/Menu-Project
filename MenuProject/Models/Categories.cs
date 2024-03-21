using System.ComponentModel.DataAnnotations;

namespace MenuProject.Models.Entities
{
    public class Categories : BaseClass
    {
        [Required]
        public string CategoryName { get; set; }
        public List<Products>? Products { get; set; }
    }
}
