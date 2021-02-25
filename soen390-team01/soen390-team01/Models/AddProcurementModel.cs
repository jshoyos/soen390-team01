using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class AddProcurementModel
    {
        [Required]
        [Display(Name = "Item ID")]
        public long ItemId { get; set; }
        [Required]
        [Display(Name = "Item Type")]
        public string ItemType{ get; set; }
        [Required]
        [Display(Name = "Item Quantity")]
        public int ItemQuantity { get; set; }
        [Required]
        [Display(Name = "Vendor ID")]
        public long VendorId { get; set; }
    }
}
