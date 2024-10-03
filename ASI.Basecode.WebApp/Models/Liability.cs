using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class Liability
    {
        public int LiabilityId { get; set; }
        public string LiabilityName { get; set; }
        public decimal LiabilityAmount { get; set; }
        public string LiabilityColor { get; set; }
        public string LiabilityIcon { get; set; }
        public string LiabilityDest { get; set; }
        public string LiabilityHappDate { get; set; }
        public string LiabilityDueDate { get; set; }
    }

}
