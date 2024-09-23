using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Check if _context and _categoryService are not null
                if (_context == null)
                {
                    return BadRequest("Database context is not available.");
                }

                if (category == null)
                {
                    return BadRequest("Category data is missing.");
                }

                // Add category to the database
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Return the new category data as JSON to be used in the client-side JavaScript
                return Json(new
                {
                    id = category.Id,
                    name = category.Name,
                    type = category.Type,
                    icon = category.Icon,
                    color = category.Color
                });
            }

            // Return an error message if model validation fails
            return BadRequest("Failed to create category.");
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
