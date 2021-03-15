using soen390_team01.Data.Entities;
using System.Collections.Generic;

namespace soen390_team01.Models
{ 
    public class UserManagementModel
    {
        public AddUserModel AddUser { get; set; }
        public EditUserModel EditUser { get; set; }
        public List<User> Users { get; set; }
    }
}
