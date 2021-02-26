#region Header

// Author: Tommy Andrews
// File: UserManagementModel.cs
// Project: soen390-team01
// Created: 02/25/2021
// 

#endregion

using System.Collections.Generic;
using soen390_team01.Data.Entities;

namespace soen390_team01.Models
{
    public class UserManagementModel
    {
        public AddUserModel AddUser { get; set; }
        public List<User> Users { get; set; }
    }
}