using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } 

        [Required(ErrorMessage = "Account is required")]
        public int DeLiId { get; set; }

        public string DebitLiabilityName { get; set; } 

        public int UserId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string TransactionType { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime TransactionDate { get; set; }

        public string Note { get; set; }
    }
}
