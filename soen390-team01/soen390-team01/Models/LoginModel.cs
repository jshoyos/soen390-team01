using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class LoginModel
    {
        #region properties
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        #endregion
    }
}
