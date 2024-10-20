using System.Collections.Generic;

namespace ProductCategoryApp.Models
{
    public class ProductCategoryViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
