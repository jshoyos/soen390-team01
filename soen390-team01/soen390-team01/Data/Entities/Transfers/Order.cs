using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public long OrderId { get; set; }
        public long CustomerId { get; set; }
        public string State { get; set; }
        public long PaymentId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
