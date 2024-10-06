using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "UserCode is required")]
        public string UserCode { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        // Email
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Mail { get; set; }  // You can rename this to Email if preferred

        // Add Password Reset Token
        public string PasswordResetToken { get; set; }

        // Add Password Reset Expiration
        public DateTime? PasswordResetExpiration { get; set; }
    }

    public class UserListViewModel
    {
        [DisplayName("ID")]
        public string IdFilter { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(Resources.Views.Screen))]
        public string FirstNameFilter { get; set; }

        public IEnumerable<UserViewModel> dataList { get; set; }
    }
}
