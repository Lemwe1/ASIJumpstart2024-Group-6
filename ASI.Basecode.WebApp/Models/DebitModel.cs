using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class DebitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public decimal Balance { get; set; }
        public string Color { get; set; }
        public bool IsBorrowed { get; set; }
    }


}
