using System;
using System.Collections.Generic;

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

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Procurement> Procurements { get; set; }
    }
}
