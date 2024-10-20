using System.ComponentModel.DataAnnotations;

namespace ProductCategoryApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

       [Required(ErrorMessage = "Category Name is required.")]
    public string CategoryName { get; set; } // Category Name
    }
}
