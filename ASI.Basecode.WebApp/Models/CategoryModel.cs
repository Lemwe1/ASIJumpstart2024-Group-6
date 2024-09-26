using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Icon { get; set; }

        [Required]
        public string Color { get; set; }
    }
}
