using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BudgetViewModel
    {
        public int? BudgetId { get; set; } // Nullable for new budgets

        [Required(ErrorMessage = "Category is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryIcon { get; set; }

        public string CategoryColor { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Monthly budget amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Monthly budget must be greater than zero.")]
        public decimal MonthlyBudget { get; set; }

        public decimal RemainingBudget { get; set; } // Automatically calculated
    }


}