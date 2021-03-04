using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace soen390_team01.Models
{
    public class AccountingModel
    {
        public List<Payment> PaymentList { get; set; }

        public string SelectedTab { get; set; } = "All";
    }
}
