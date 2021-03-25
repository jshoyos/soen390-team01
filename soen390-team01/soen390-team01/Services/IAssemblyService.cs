using soen390_team01.Data.Entities;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public interface IAssemblyService : IFilteredModel
    {
        public List<Production> Productions { get; set; }
        public Filters ProductionFilters { get; set; }
        public BikeOrder BikeOrder { get; set; }
        public string SelectedTab { get; set; }
        public bool ShowModal { get; set; }
        public List<Production> GetProductions();
        public Filters ResetProductionFilters();
        public List<Production> GetFilteredProductionList(Filters filters);
        public Production AddNewBike(BikeOrder order);
    }
}
