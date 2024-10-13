using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Category List";

            // Get user ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            // Convert the string userId to an integer
            if (!int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid user ID");
            }

            // Retrieve list of MCategory from the service for the specific user
            var categories = await _categoryService.GetCategoriesAsync(userId);

            // Map MCategory to CategoryViewModel
            var categoryViewModels = categories.Select(category => new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Type = category.Type,
                Icon = category.Icon,
                Color = category.Color
            }).ToList();

            return View(categoryViewModels);
        }


        // GET: /Category/GetCategory/{id}
        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            // Get user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id, userId);
            if (category == null)
            {
                return NotFound(new { success = false, message = "Category not found." });
            }

            // Map MCategory to CategoryViewModel
            var categoryViewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Type = category.Type,
                Icon = category.Icon,
                Color = category.Color
            };

            return Json(new { success = true, data = categoryViewModel });
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Please Fill Up Everything", errors });
            }

            try
            {
                // Get user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Map CategoryViewModel to MCategory
                var category = new MCategory
                {
                    Name = categoryViewModel.Name,
                    Type = categoryViewModel.Type,
                    Icon = categoryViewModel.Icon,
                    Color = categoryViewModel.Color
                };

                await _categoryService.AddCategoryAsync(category, userId);
                return Json(new { success = true, message = "Category created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: /Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel categoryViewModel)
        {
            if (id != categoryViewModel.CategoryId)
            {
                return BadRequest(new { success = false, message = "Invalid category ID." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Invalid data", errors });
            }

            try
            {
                // Get user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Map CategoryViewModel to MCategory
                var category = new MCategory
                {
                    CategoryId = categoryViewModel.CategoryId,
                    Name = categoryViewModel.Name,
                    Type = categoryViewModel.Type,
                    Icon = categoryViewModel.Icon,
                    Color = categoryViewModel.Color
                };

                await _categoryService.UpdateCategoryAsync(category, userId);
                return Json(new { success = true, message = "Category updated successfully." });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
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
                // Get user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                await _categoryService.DeleteCategoryAsync(id, userId);
                return Json(new { success = true, message = "Category deleted successfully." });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
