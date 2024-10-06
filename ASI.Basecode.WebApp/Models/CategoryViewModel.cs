using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } // Could be an enum for Expense/Income

        [Required(ErrorMessage = "Icon is required.")]
        public string Icon { get; set; } // Icon URL or name, depends on your implementation

        [Required(ErrorMessage = "Color is required.")]
        public string Color { get; set; } // A string representing the color code (e.g., #FFFFFF)
    }
}
