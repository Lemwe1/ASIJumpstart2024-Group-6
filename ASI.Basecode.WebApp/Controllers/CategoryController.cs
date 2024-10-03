using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: /Category/
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Category List";
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
                return NotFound(new { success = false, message = "Category not found." });
            }
            return Json(new { success = true, data = category });
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                Console.WriteLine("Model errors: " + string.Join(", ", errors));
                return BadRequest(new { success = false, message = "Invalid data", errors });
            }

            try
            {
                await _categoryService.AddCategoryAsync(category);
                return Json(new { success = true, message = "Category created successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating category: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: /Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest(new { success = false, message = "Invalid category ID." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                Console.WriteLine("Model errors: " + string.Join(", ", errors));
                return BadRequest(new { success = false, message = "Invalid data", errors });
            }

            try
            {
                await _categoryService.UpdateCategoryAsync(category);
                return Json(new { success = true, message = "Category updated successfully." });
            }
            catch (KeyNotFoundException knfEx)
            {
                Console.WriteLine(knfEx.Message);
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: /Category/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return Json(new { success = true, message = "Category deleted successfully." });
            }
            catch (KeyNotFoundException knfEx)
            {
                Console.WriteLine(knfEx.Message);
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
