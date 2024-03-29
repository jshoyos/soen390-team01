﻿using soen390_team01.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class EditUserModel : User
    {
        #region properties
        [Required]
        public override string Role { get; set; }
        [Display(Name = "Phone Number")]
        [Required]
        [StringLength(10)]
        [DisplayFormat(DataFormatString = "{0:###-###-####}", ApplyFormatInEditMode = true)]
        [DataType(DataType.PhoneNumber)]
        public override string PhoneNumber { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public override string LastName { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public override string FirstName { get; set; }
        #endregion

        public EditUserModel()
        {
            Email = "";
            PhoneNumber = "";
            LastName = "";
            FirstName = "";
        }
        public EditUserModel(User user)
        {
            Email = user.Email;
            Role = user.Role;
            PhoneNumber = user.PhoneNumber;
            LastName = user.LastName;
            FirstName = user.FirstName;
            UserId = user.UserId;
        }
    }
}
