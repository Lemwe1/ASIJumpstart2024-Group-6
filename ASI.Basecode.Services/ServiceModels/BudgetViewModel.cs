using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BudgetViewModel
    {
        public int BudgetId { get; set; }

        [Required(ErrorMessage = "Budget name is required.")]
        public string BudgetName { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } // Optional for UI display

        public string CategoryIcon { get; set; } // Optional for UI display

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Monthly budget amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Monthly budget must be a positive number.")]
        public decimal MonthlyBudget { get; set; }

        public decimal RemainingBudget { get; set; } // This will be updated as transactions are added
    }
}
