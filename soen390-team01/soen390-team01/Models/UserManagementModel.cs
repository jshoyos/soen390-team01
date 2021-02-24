using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{ 
    public class UserManagementModel
    {
        public AddUserModel AddUser { get; set; }
        public List<User> Users { get; set; }
    }
}
