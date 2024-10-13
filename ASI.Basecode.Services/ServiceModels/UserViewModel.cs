using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UserCode is required")]
        public string UserCode { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]  // Ensure password is treated securely
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Mail { get; set; }

        // Fields for email verification functionality
        public string VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiration { get; set; }

        // Verification status
        public bool isVerified { get; set; }  // Track email verification status

        // Fields for password reset functionality
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiration { get; set; }

        // Other optional fields
        public string Name { get; set; }  // This could be derived from FirstName + LastName if needed
        public string Description { get; set; }
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
