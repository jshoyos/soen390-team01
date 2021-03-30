using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using soen390_team01.Data.Queries;
using soen390_team01.Data.Exceptions;
using soen390_team01.Services;

namespace soen390_team01.Models
{
    public class AccountingModel: FilteredModel, IAccountingService
    {
        private readonly ErpDbContext _context;
        public List<Payment> Payments { get; set; }
        public List<Payment> Receivables { get; set; }
        public List<Payment> Payables { get; set; }
        public Filters PaymentFilters { get; set; }
        public Filters ReceivableFilters { get; set; }
        public Filters PayableFilters { get; set; }
        public string SelectedTab { get; set; } = "payment";

        private static readonly List<string> StatusValues = new() { "pending", "completed", "canceled" };
    

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
            return _context.Payments.ToList();
        }

        public List<Payment> GetReceivables()
        {
            return _context.Payments.Where(p => p.Amount > 0).ToList();
        }

        public List<Payment> GetPayables()
        {
            return _context.Payments.Where(p => p.Amount < 0).ToList();

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
            var filters = new Filters("payment", tabName);

            filters.Add(new CheckboxFilter("payment", "State", "state", StatusValues));
            filters.Add(new RangeFilter("payment", "Amount", "amount"));
            filters.Add(new DateRangeFilter("payment", "Added", "added"));
            filters.Add(new DateRangeFilter("payment", "Updated", "modified"));

            return filters;
        }

        public void FilterSelectedTab(Filters filters)
        {
            switch (filters.Tab)
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
                return _context.Payments.FromSqlRaw(PaymentQueryBuilder.FilterPayment(filters, condition)).ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }

        public void setReceivableState(int id, string status)
        {
            for(var i = 0; i<Receivables.Count; i++)
            {
                if (id == Receivables[i].PaymentId)
                {
                    Receivables[i].State = status;
                }
            }
        }
    }   
}
