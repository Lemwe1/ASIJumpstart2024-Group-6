using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "DeLiId is required")]
        public int DeLiId { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; } // UserId field added

        [Required(ErrorMessage = "TransactionType is required")]
        public string TransactionType { get; set; } // TransactionType field added

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "TransactionDate is required")]
        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }
        public string Note { get; set; }
    }
}
