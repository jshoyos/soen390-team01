using System;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using soen390_team01.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using soen390_team01.Data.Exceptions;
using Npgsql;
using soen390_team01.Services;
using soen390_team01.Models;
using soen390_team01.Data.Queries;
using System.Globalization;

namespace soen390_team01.Models
{
    public class TransfersModel : ITransferService
    {
        private readonly ErpDbContext _context;
        public List<Order> Orders { get; set; }
        public List<Procurement> Procurements { get; set; }
        public AddProcurementModel AddProcurement { get; set; }

        //Filter list
        public Filters ProcurementFilters { get; set; }
        public Filters OrderFilters { get; set; }

        public string SelectedTab { get; set; } = "Order";
        public bool ShowModal { get; set; } = false;

        public TransfersModel(){}

        public TransfersModel(ErpDbContext context)
        {
            _context = context;
            Procurements = GetProcurements();
            Orders = GetOrders();
            ProcurementFilters = ResetProcurementFilters();
            OrderFilters = ResetOrderFilters();
        }

        public List<Procurement> GetProcurements()
        {
            List<Procurement> list = _context.Procurements
                    .Include(p => p.Payment)
                    .Include(p => p.Vendor)
                    .ToList();
            return list;
        }
        public List<Order> GetOrders()
        {
            List<Order> list = _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Payment)
                    .Include(o => o.Customer)
                    .ToList();
            return list;
        }

        public Filters ResetProcurementFilters()
        {
            var filters = new Filters("procurement");

            filters.Add(new SelectFilter("procurement", "Vendor", "vendor", _context.Procurements.Select(procurement => procurement.Vendor.Name).Distinct().OrderBy(v => v).ToList()));
            filters.Add(new CheckboxFilter("procurement", "State", "state", _context.Procurements.Select(procurement => procurement.State).Distinct().OrderBy(s => s).ToList()));
            filters.Add(new DateRangeFilter("procurement", "Added", "added"));
            filters.Add(new DateRangeFilter("procurement", "Updated", "modified"));
            return filters;
        }

        public Filters ResetOrderFilters()
        {
            var filters = new Filters("order");

            filters.Add(new CheckboxFilter("order", "Status", "state", _context.Procurements.Select(procurement => procurement.State).Distinct().OrderBy(s => s).ToList()));
            filters.Add(new DateRangeFilter("order", "Added", "added"));
            filters.Add(new DateRangeFilter("order", "Updated", "modified"));
            return filters;
        }

        public List<Procurement> GetFilteredProcurementList(Filters filters)
        {
            try
            {
                return _context.Procurements.FromSqlRaw(TransfersQueryBuilder.FilterProcurement(filters)).ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }
        public List<Order> GetFilteredOrderList(Filters filters)
        {
            try
            {
                return _context.Orders.FromSqlRaw(TransfersQueryBuilder.FilterOrder(filters)).ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }

        /// <summary>
        /// Updates an order's status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Order ChangeOrderState(long orderId, string state)
        {
            try
            {
                ValidateState(state);
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (order == null)
                {
                    throw new NotFoundException("Order", "ID", orderId.ToString());
                }
                order.State = state;

                _context.Orders.Update(order);
                _context.SaveChanges();
                return order;
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        /// <summary>
        /// Updates a procurement's status
        /// </summary>
        /// <param name="procurementId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Procurement ChangeProcurementState(long procurementId, string state)
        {
            try
            {
                ValidateState(state);
                var procurement = _context.Procurements.FirstOrDefault(o => o.ProcurementId == procurementId);
                if (procurement == null)
                {
                    throw new NotFoundException("Procurement", "ID", procurementId.ToString());
                }
                procurement.State = state;

                _context.Procurements.Update(procurement);
                _context.SaveChanges();
                return procurement;
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        /// <summary>
        /// Inserting a new procurement
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="addProcurements">procurement insertion input</param>
        /// <returns></returns>
        public Procurement AddProcurements<T>(AddProcurementModel addProcurement) where T : Item
        {
            try
            {
                var item = _context.Set<T>("soen390_team01.Data.Entities." + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addProcurement.ItemType))
                    .FromSqlRaw(ProductQueryBuilder.GetProduct(addProcurement.ItemType, addProcurement.ItemId))
                    .First();

                var payment = new Payment
                {
                    Amount = item.Price * addProcurement.ItemQuantity,
                    State = "pending"
                };

                _context.Payments.Add(payment);
                _context.SaveChanges();

                var procurement = new Procurement
                {
                    ItemId = item.ItemId,
                    Type = addProcurement.ItemType.ToLower(),
                    ItemQuantity = addProcurement.ItemQuantity,
                    VendorId = addProcurement.VendorId,
                    PaymentId = payment.PaymentId,
                    State = "pending"
                };
                _context.Procurements.Add(procurement);

                _context.SaveChanges();

                Procurements = GetProcurements();

                return procurement;
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        private static void ValidateState(string state)
        {
            if (!TransferState.In(state))
            {
                throw new InvalidValueException("state", state);
            }
        }
    }
    public static class TransferState
    {
        private static readonly List<string> States;
        static TransferState()
        {
            States = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };
        }

        public static bool In(string value)
        {
            return States.Contains(value);
        }
    }
}
