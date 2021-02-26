#region Header

// Author: Tommy Andrews
// File: User.cs
// Project: soen390-team01
// Created: 02/23/2021
// 

#endregion

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class User
    {
        public User()
        {
        }

        public User(User u)
        {
            Role = u.Role;
            PhoneNumber = u.PhoneNumber;
            LastName = u.LastName;
            FirstName = u.FirstName;
            Email = u.Email;
            Iv = u.Iv;
        }

        public virtual string Role { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Email { get; set; }
        public long UserId { get; set; }
        public string Iv { get; set; }
    }
}