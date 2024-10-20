using System.ComponentModel.DataAnnotations;

namespace ProductCategoryApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }
        
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
