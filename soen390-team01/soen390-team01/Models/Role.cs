﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Models
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string Accountant = "Accountant";
        public const string InventoryManager = "InventoryManager";
        public const string SalesRep = "SalesRep";
    }
    public enum Roles
    {
        [Display(Name = "Admin")]
        Admin,
        Accountant,
        [Display(Name = "Inventory Manager")]
        InventoryManager,
        [Display(Name = "Sales Representative")]
        SalesRep,
        [Display(Name = "")]
        None
    }
}
