using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;  // Injected context
        private readonly CategoryService _categoryService;

        public CategoryController(ApplicationDbContext context, CategoryService categoryService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: /Category/
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Category"; // Setting the title for the page
            var categories = await _categoryService.GetCategoriesAsync();
            return View(categories);
        }

        // GET: /Category/GetCategory/{id}
        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Json(category);
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Add the category to the database
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Return the new category data to the client
                return Json(new
                {
                    id = category.Id,
                    name = category.Name,
                    type = category.Type,
                    icon = category.Icon,
                    color = category.Color
                });
            }

            // Return a bad request if model validation fails
            return BadRequest(new { success = false, message = "Failed to create category.", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        // POST: /Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.Id)
            {
                return BadRequest(new { success = false, message = "Invalid category ID." });
            }

            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == category.Id);
                if (existingCategory == null)
                {
                    return NotFound(new { success = false, message = "Category not found." });
                }

                try
                {
                    _context.Entry(category).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Category updated successfully." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = ex.Message });
                }
            }

            // Return validation errors
            return BadRequest(new { success = false, message = "Invalid category data.", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        // POST: /Category/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
