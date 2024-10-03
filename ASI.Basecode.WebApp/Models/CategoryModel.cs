using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; } // "Expense" or "Income"

        [Required]
        public string Icon { get; set; } // e.g., "fas fa-shopping-bag"

        [Required]
        public string Color { get; set; } // e.g., "#000000"
    }
}
