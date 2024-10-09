using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class DebitLiabilityViewModel
    {
        public int DeLiId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string DeLiType { get; set; }

        [Required(ErrorMessage = "Balance is required")]
        public decimal DeLiBalance { get; set; }

        public string DeLiIcon { get; set; }
        public string DeLiColor { get; set; }
        public DateTime? DeLiHapp { get; set; }
        public DateTime? DeLiDue { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string DeLiName { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
