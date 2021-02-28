﻿using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public static class TransferState
    {
        private static readonly List<string> States;
        static TransferState()
        {
            States = new List<string>
            {
                "pending",
                "completed",
                "cancelled"
            };
        }

        public static bool In(string value)
        {
            return States.Contains(value);
        }
    }

    public class TransfersService : DisposableService
    {
        private readonly ErpDbContext _context;

        public TransfersService(ErpDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Queries all orders and procurements and adds them to a TransfersModel
        /// </summary>
        /// <returns>InventoryModel</returns>
        public virtual TransfersModel GetTransfersModel()
        {
            return new()
            {
                Orders = _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Payment)
                    .Include(o => o.Customer)
                    .ToList(),
                Procurements = _context.Procurements
                    .Include(p => p.Payment)
                    .Include(p => p.Vendor)
                    .ToList()
            };
        }

        /// <summary>
        /// Updates an order's status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public virtual Order ChangeOrderState(long orderId, string state) 
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
        public virtual Procurement ChangeProcurementState(long procurementId, string state)
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
        /// <param name="addProcurement">procurement insertion input</param>
        /// <returns></returns>
        public virtual Procurement AddProcurement<T>(AddProcurementModel addProcurement) where T : Item
        {
            try
            {
                var item = _context.Set<T>("soen390_team01.Data.Entities." + addProcurement.ItemType)
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
}