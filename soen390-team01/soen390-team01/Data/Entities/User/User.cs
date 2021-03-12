using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class User
    {
        public User() { }
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
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }
    }
}
