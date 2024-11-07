using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class WalletViewModel
    {
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Balance is required")]
        public decimal WalletBalance { get; set; }

        public string WalletIcon { get; set; }
        public string WalletColor { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string WalletName { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
