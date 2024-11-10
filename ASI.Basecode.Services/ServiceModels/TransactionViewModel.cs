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

        public string CategoryIcon { get; set; }

        public string CategoryColor { get; set; }

        [Required(ErrorMessage = "Wallet is required")]
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string WalletIcon { get; set; }
        public string WalletColor { get; set; }

        public decimal WalletBalance { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Transaction Type is required")]
        public string TransactionType { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }


        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }

        public string Note { get; set; }

        public string TransactionSort { get; set; }


    }
}
