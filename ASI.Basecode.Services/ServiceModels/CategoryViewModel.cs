using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Services.ServiceModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Icon is required")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; }

        public string UserId { get; set; }
    }
}
