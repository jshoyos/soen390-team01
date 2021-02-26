#region Header

// Author: Tommy Andrews
// File: Bike.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Bike : Item
    {
        public Bike()
        {
            BikeParts = new HashSet<BikePart>();
        }

        public string Size { get; set; }

        public virtual ICollection<BikePart> BikeParts { get; set; }
    }
}