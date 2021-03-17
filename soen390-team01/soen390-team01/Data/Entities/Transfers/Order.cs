using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Added { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
