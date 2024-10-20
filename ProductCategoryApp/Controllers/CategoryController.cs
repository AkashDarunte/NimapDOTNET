using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategoryApp.Models;
using System;
using System.Linq;

namespace ProductCategoryApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Category/
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category input)
        {
            if (string.IsNullOrWhiteSpace(input?.CategoryName))
            {
                ModelState.AddModelError("", "Category name cannot be empty.");
                return View(input);
            }

            try
            {
                // Check if the category already exists by comparing names 
                var existingCategory = _context.Categories
                    .FirstOrDefault(c => c.CategoryName.ToLower() == input.CategoryName.ToLower());

                if (existingCategory == null)
                {
                    _context.Categories.Add(input);
                    _context.SaveChanges();

                    // Use TempData to pass success message
                    TempData["SuccessMessage"] = "Category successfully created!";
                    return RedirectToAction("Index");
                }
                else
                {
                    // If category already exists, add error to the ModelState
                    ModelState.AddModelError("", $"{input.CategoryName} already exists.");
                    return View(input);
                }
            }
            catch (Exception ex)
            {
                // Catch any database errors
                ModelState.AddModelError("", $"An error occurred while saving the category: {ex.Message}");
                return View(input);
            }
        }



        // GET: Category/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        
        if (category == null)
        {
            return NotFound(); // Return 404 if the category doesn't exist
        }

        return View(category); // Pass the category to the view for confirmation
    }

    // POST: Category/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound(); // Return 404 if the category doesn't exist
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); // Redirect to Index after successful deletion
    }
    


    // GET: Category/Edit/{id}
public async Task<IActionResult> Edit(int id)
{
    var category = await _context.Categories.FindAsync(id);
    
    if (category == null)
    {
        return NotFound(); // Return 404 if the category doesn't exist
    }

    ViewBag.Categories = await _context.Categories.ToListAsync(); 
    return View(category); 
}


    // POST: Category/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Entry(category).State = EntityState.Modified; // Mark the category as modified
            await _context.SaveChangesAsync(); // Save changes to the database
            return RedirectToAction(nameof(Index)); // Redirect to Index after successful update
        }
        return View(category); // Return to the view if the model is invalid
    }


    }
}
