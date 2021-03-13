using soen390_team01.Data.Entities;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public interface ITransferService
    {

        public List<Order> Orders { get; set; }
        public List<Procurement> Procurements { get; set; }
        public AddProcurementModel AddProcurement { get; set; }

        //Filter list
        public Filters ProcurementFilters { get; set; } 
        public Filters OrderFilters { get; set; } 

        public string SelectedTab { get; set; }
        public bool ShowModal { get; set; }

        public Filters ResetProcurementFilters();

        public Filters ResetOrderFilters();

        public Procurement AddProcurements<T>(AddProcurementModel addProcurement) where T : Item;
        List<Procurement> GetFilteredProcurementList(Filters filters);
        List<Procurement> GetProcurements();
        List<Order> GetFilteredOrderList(Filters filters);
        List<Order> GetOrders();
        TransfersModel SetupModel();
    }
}
