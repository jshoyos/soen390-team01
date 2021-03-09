using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Queries;

namespace soen390_team01.Services
{
    public interface IAccountingService
    {
        public List<Payment> Payments { get; set; }
        public List<Payment> Receivables { get; set; }
        public List<Payment> Payables { get; set; }
        public Filters PaymentFilters { get; set; }
        public Filters ReceivableFilters { get; set; }
        public Filters PayableFilters { get; set; }
        public string SelectedTab { get; set; }

        public List<Payment> GetPayments();

        public List<Payment> GetReceivables();

        public List<Payment> GetPayables();
    }

}
