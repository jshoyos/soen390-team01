using soen390_team01.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class AddUserModel : User
    {

        public AddUserModel()
        {
            Email = "";
            Role = "";
            PhoneNumber = "";
            LastName = "";
            FirstName = "";
            Password = "";
            ConfirmPassword = "";
        }
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        public override string Email { get; set; }
        [Required]
        public override string Role { get; set; }
        [Display(Name = "Phone Number")]
        [Required]
        [StringLength(10)]
        [DisplayFormat(DataFormatString ="{0:###-###-####}", ApplyFormatInEditMode = true)]
        [DataType(DataType.PhoneNumber)]
        public override string PhoneNumber { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public override string LastName { get; set; }
        [Display(Name = "First Name")]
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
