using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ASI.Basecode.WebApp.Models
{
    /// <summary>
    /// Register View Model
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>User Code</summary>
        [JsonPropertyName("userCode")]
        [Required(ErrorMessage = "UserCode is required.")]
        public string UserCode { get; set; }

        /// <summary>Password</summary>
        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>Confirm Password</summary>
        [JsonPropertyName("confirmPassword")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        /// <summary>First Name</summary>
        [JsonPropertyName("firstName")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        /// <summary>Last Name</summary>
        [JsonPropertyName("lastName")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        /// <summary>Email</summary>
        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
    }
}
