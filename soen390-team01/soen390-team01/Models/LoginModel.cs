#region Header

// Author: Tommy Andrews
// File: LoginModel.cs
// Project: soen390-team01
// Created: 02/04/2021
// 

#endregion

using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class LoginModel
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}