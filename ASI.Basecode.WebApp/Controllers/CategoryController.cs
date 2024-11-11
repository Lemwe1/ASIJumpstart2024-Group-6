using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize] // Ensure that the user is authenticated for all actions
    [Route("Category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: /Category
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Category List";

            // Get user ID as string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Convert userIdString to int?
            int? userId = null;
            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            // Retrieve list of MCategory for both user-specific and global categories
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
        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var userId = GetUserId();
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
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Please fill up everything.", errors });
            }

            try
            {
                var userId = GetUserId();

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
        [HttpPost("Edit/{id}")]
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
                return BadRequest(new { success = false, message = "Invalid data.", errors });
            }

            try
            {
                var userId = GetUserId();

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
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();

                await _categoryService.DeleteCategoryAsync(id, userId);
                return Json(new { success = true, message = "Category deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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

        // Helper method to retrieve the user ID from claims
        private int? GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int parsedUserId))
            {
                return parsedUserId;
            }
            return null;
        }
    }
}
