using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using soen390_team01.Data.Queries;

namespace soen390_team01.Models
{
    public class AccountingModel
    {
        private readonly ErpDbContext _context;
        public List<Payment> Payments { get; set; }
        public List<Payment> Receivables { get; set; }
        public List<Payment> Payables { get; set; }
        public Filters PaymentFilters { get; set; }
        public Filters ReceivableFilters { get; set; }
        public Filters PayableFilters { get; set; }
        public string SelectedTab { get; set; } = "All";

        private static readonly List<string> StatusValues = new List<string> { "Pending", "Completed", "Cancelled" };
    

        public AccountingModel(ErpDbContext context)
        {
            _context = context;
            PaymentFilters = ResetFilters();
            ReceivableFilters = ResetFilters();
            PayableFilters = ResetFilters();
        }

        public List<Payment> GetPayments()
        {
            List<Payment> list = _context.Payments.ToList();
            return list;
        }

        public List<Payment> GetReceivables()
        {
            List<Payment> list = _context.Payments.ToList();
            List<Payment> receivablesList = new List<Payment>();
            foreach (Payment p in list)
            {
                if (p.Amount > 0)
                {
                    receivablesList.Add(p);
                }
            }
            return receivablesList;
        }

        public List<Payment> GetPayables()
        {
            List<Payment> list = _context.Payments.ToList();
            List<Payment> payablesList = new List<Payment>();
            foreach (Payment p in list)
            {
                if (p.Amount < 0)
                {
                    payablesList.Add(p);
                }
            }
            return payablesList;
        }

        private Filters ResetFilters()
        {
            var filters = new Filters("payment");

            filters.Add(new CheckboxFilter("payment", "Status", "status", StatusValues));
            filters.Add(new RangeFilter("payment", "Amount", "amount"));

            return filters;
        }

    }   
}
