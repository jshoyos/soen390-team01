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
        private readonly IProductionService _productionService;

        public List<Production> Productions { get; set; }
        public Filters ProductionFilters { get; set; }
        public BikeOrder BikeOrder { get; set; }
        public string SelectedTab { get; set; } = "production";
        public bool ShowModal { get; set; } = false;

        private static readonly List<string> StatusValues = new() { "stopped", "inProgress", "completed" };
        public AssemblyModel() { }

        public AssemblyModel(ErpDbContext context, IProductionService productionService)
        {
            _productionService = productionService;
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

        public void AddNewBike(BikeOrder order)
        {
            try
            {
                Bike bike = _context.Bikes.First(b => b.ItemId == order.BikeId);
                _productionService.ProduceBike(bike,order.ItemQuantity);

                Productions = GetProductions();
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        public Inventory UpdateInventory(Production production)
        {
            try
            {
                Inventory updatedInventory = null;
                try
                {
                     updatedInventory = _context.Inventories.First(i => i.ItemId == production.BikeId);
                }
                catch(Exception e)
                {

                }

                if (updatedInventory == null) //checks if bike exist in inventory
                {
                    Inventory inventory = new Inventory
                    {
                        ItemId = production.BikeId,
                        Quantity = production.Quantity,
                        Type = "bike",
                        Warehouse = "Warehouse 1" //Default warehouse
                    };
                    _context.Inventories.Add(inventory);
                }
                else
                {
                    updatedInventory.Quantity += production.Quantity;
                    _context.Inventories.Update(updatedInventory);
                }

                _context.SaveChanges();
                return updatedInventory;
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        public Production UpdateProduction(Production production)
        {
            try
            {
                var updatedProduction = _context.Productions.First(i => i.ProductionId == production.ProductionId);
                updatedProduction.State = production.State;
                _context.Productions.Update(updatedProduction);
                _context.SaveChanges();
                return updatedProduction;
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }

        }


    }
}
