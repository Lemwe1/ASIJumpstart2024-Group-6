using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class DebitAccount
    {
        public int DebitId { get; set; }
        public string DebitName { get; set; }
        public string DebitIcon { get; set; }
        public decimal DebitBalance { get; set; }
        public string DebitColor { get; set; }
    }

}
