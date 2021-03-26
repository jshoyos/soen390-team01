using System;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using soen390_team01.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using soen390_team01.Data.Exceptions;
using soen390_team01.Services;
using soen390_team01.Data.Queries;
using Npgsql;

namespace soen390_team01.Models
{
    public class AssemblyModel : FilteredModel, IAssemblyService
    {
        private readonly ErpDbContext _context;

        public List<Production> Productions { get; set; }
        public Filters ProductionFilters { get; set; }
        public BikeOrder BikeOrder { get; set; }
            
        public string SelectedTab { get; set; } = "production";
        public bool ShowModal { get; set; } = false;

        private static readonly List<string> StatusValues = new() { "pending", "completed", "canceled" };
        public AssemblyModel() { }

        public AssemblyModel(ErpDbContext context)
        {
            _context = context;
            Productions = GetProductions();
            ProductionFilters = ResetProductionFilters();
        }
        public List<Production> GetProductions()
        {
            List<Production> list = _context.Productions.ToList();
            return list;
        }

        public Filters ResetProductionFilters()
        {
            var filters = new Filters("production");

            filters.Add(new CheckboxFilter("production", "State", "state", StatusValues));
            filters.Add(new DateRangeFilter("production", "Added", "added"));
            filters.Add(new DateRangeFilter("production", "Updated", "modified"));
            return filters;
        }

        public List<Production> GetFilteredProductionList(Filters filters)
        {
            try
            {
                return _context.Productions.FromSqlRaw(ProductQueryBuilder.FilterProduct(filters)).ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }

        public Production AddNewBike(BikeOrder order)
        {
            try
            {
                var production = new Production
                {
                    BikeId = order.BikeId,
                    Quantity = order.ItemQuantity,
                    State = "pending"
                };
                _context.Productions.Add(production);
                _context.SaveChanges();

                Productions = GetProductions();
                return production;
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }
    

    }
}
