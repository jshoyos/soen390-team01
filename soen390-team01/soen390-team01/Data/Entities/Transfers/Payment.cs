using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
            Procurements = new HashSet<Procurement>();
        }

        public long PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Added { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Procurement> Procurements { get; set; }        
    }
}
