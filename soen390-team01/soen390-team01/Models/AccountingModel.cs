using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using soen390_team01.Data.Queries;
using System.Globalization;
using soen390_team01.Data.Exceptions;
using soen390_team01.Services;

namespace soen390_team01.Models
{
    public class AccountingModel: IAccountingService
    {
        private readonly ErpDbContext _context;
        public List<Payment> Payments { get; set; }
        public List<Payment> Receivables { get; set; }
        public List<Payment> Payables { get; set; }
        public Filters PaymentFilters { get; set; }
        public Filters ReceivableFilters { get; set; }
        public Filters PayableFilters { get; set; }
        public string SelectedTab { get; set; } = "payment";

        private static readonly List<string> StatusValues = new List<string> { "Pending", "Completed", "Canceled" };
    

        public AccountingModel(ErpDbContext context)
        {
            _context = context;
            PaymentFilters = ResetFilters("payment");
            ReceivableFilters = ResetFilters("receivable");
            PayableFilters = ResetFilters("payable");
            Payments = GetPayments();
            Receivables = GetReceivables();
            Payables = GetPayables();
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
        public void ResetPayments()
        {
            Payments = GetPayments();
            PaymentFilters = ResetFilters("payment");
        }

        public void ResetPayables()
        {
            Payables = GetPayables();
            PayableFilters = ResetFilters("payable");
        }

        public void ResetReceivables()
        {
            Receivables = GetReceivables();
            ReceivableFilters = ResetFilters("receivable");
        }

        private Filters ResetFilters(string tabName)
        {
            var filters = new Filters("payment");

            filters.Add(new CheckboxFilter("payment", $"State-{tabName}", "state", StatusValues));
            filters.Add(new RangeFilter("payment", $"Amount-{tabName}", "amount"));
            filters.Add(new DateRangeFilter("payment", $"Added-{tabName}", "added"));
            filters.Add(new DateRangeFilter("payment", $"Updated-{tabName}", "modified"));

            return filters;
        }

        public void FilterSelectedTab(Filters filters)
        {
            switch (SelectedTab)
            {
                case "receivable":
                    Receivables = filters.AnyActive() ? GetFilteredPaymentList(filters, " and amount >= '0.0'") : GetReceivables();
                    ReceivableFilters = filters;
                    break;
                case "payable":
                    Payables = filters.AnyActive() ? GetFilteredPaymentList(filters, " and amount <= '0.0'") : GetPayables();
                    PayableFilters = filters;
                    break;
                case "payment":
                    Payments = filters.AnyActive() ? GetFilteredPaymentList(filters, "") : GetPayments();
                    PaymentFilters = filters;
                    break;
            }
        }

        public List<Payment> GetFilteredPaymentList(Filters filters, string condition)
        {
            try
            {
                return _context.Payments
                    .FromSqlRaw(ProductQueryBuilder.FilterProduct(new Filters("payment", filters.List), condition)).ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }
    }   
}
