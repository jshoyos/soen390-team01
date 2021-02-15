using soen390_team01.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class RegisterModel : User
    {
        [Required]
        [EmailAddress]
        public override string Email { get; set; }
        [Required]
        public override string Role { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public override string PhoneNumber { get; set; }
        [Required]
        public override string LastName { get; set; }
        [Required]
        public override string FirstName { get; set; }

        [Required]
        [StringLength(999, MinimumLength = 6, ErrorMessage = "The password should be a minimum 6 characters")]
        [Compare("ConfirmPassword", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field cannot be empty")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
