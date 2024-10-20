using Microsoft.AspNetCore.Mvc;
using ProductCategoryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Product Index with Pagination
    public IActionResult Index(int pageNumber = 1, int pageSize = 10)
    {
        // Fetch total products count
        var totalProducts = _context.Products.Count();

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

        // Fetch products for the current page
        var products = _context.Products
            .Include(p => p.Category) // Include related Category
            .Skip((pageNumber - 1) * pageSize) // Skip records for previous pages
            .Take(pageSize) // Take records for the current page
            .ToList();

        
        var viewModel = new ProductCategoryViewModel
        {
            Products = products,
            Categories = _context.Categories.ToList(),
            CurrentPage = pageNumber,
            TotalPages = totalPages
        };

        return View(viewModel); // Pass the ViewModel to the view
    }

    // GET: Product/Create
    public IActionResult Create()
    {
        // Fetch categories for the dropdown
        ViewBag.Categories = _context.Categories.ToList(); 
        return View("AddProduct"); // Return the AddProduct view
    }

    // POST: Add Product
    [HttpPost]
    public async Task<IActionResult> AddProduct(Product prd)
    {
        if (ModelState.IsValid) // Check if the model is valid
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(a => a.ProductName == prd.ProductName);
            var lastProduct = await _context.Products.OrderByDescending(s => s.ProductId).FirstOrDefaultAsync();

            if (existingProduct != null)
            {
                return Conflict("Product already exists.");
            }
            else
            {
                prd.ProductId = lastProduct != null ? lastProduct.ProductId + 1 : 1;

                _context.Products.Add(prd);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Redirect to Index after successful addition
            }
        }
        ViewBag.Categories = _context.Categories.ToList(); // Fetch categories again in case of error
        return View(prd); // Return to the view if the model is invalid
    }

    // GET: Product/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            return NotFound(); // Return 404 if the product doesn't exist
        }

        return View(product); // Pass the product to the view
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return Conflict($"{id} does not exist.");
        }
        else
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok($"{id} deleted successfully"); 
        }
    }

    // GET: Product/Update/{id}
    public async Task<IActionResult> Update(int id)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            return NotFound(); // Return 404 if the product doesn't exist
        }

        ViewBag.Categories = await _context.Categories.ToListAsync(); // Fetch categories for the dropdown
        return View(product); // Pass the product to the view
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProduct(Product input)
    {
        if (ModelState.IsValid)
        {
            var prd = await _context.Products.FindAsync(input.ProductId);

            if (prd == null)
            {
                return NotFound(); // Return 404 if the product doesn't exist
            }

            prd.ProductName = input.ProductName;
            prd.CategoryId = input.CategoryId; 

            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); // Redirect to the Index after successful update
        }

        
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(input);
    }
}
