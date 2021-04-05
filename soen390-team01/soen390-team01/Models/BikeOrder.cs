using System.ComponentModel.DataAnnotations;

namespace soen390_team01.Models
{
    public class BikeOrder
    {
        [Required]
        [Range(1, long.MaxValue)]
        [Display(Name = "Item ID")]
        public long BikeId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Item Quantity")]
        public int ItemQuantity { get; set; }
    }
}
