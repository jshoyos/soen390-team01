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

        public string SelectedTab { get; set; } = "All";
    

    public AccountingModel(ErpDbContext context)
    {
        _context = context;
        
    }

    public AccountingModel SetupModel()
    {
        return this;
    }

    public virtual List<Payment> getPayments()
        {
            List<Payment> list = _context.Payments
                .Include(p => p.P)
        }


    }   
}
