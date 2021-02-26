#region Header

// Author: Tommy Andrews
// File: AddProcurementModel.cs
// Project: soen390-team01
// Created: 02/25/2021
// 

#endregion

using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class AddProcurementModel
    {
        [Required]
        [Range(1, long.MaxValue)]
        [Display(Name = "Item ID")]
        public long ItemId { get; set; }

        [Required]
        [Display(Name = "Item Type")]
        public string ItemType { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Item Quantity")]
        public int ItemQuantity { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        [Display(Name = "Vendor ID")]
        public long VendorId { get; set; }
    }
}