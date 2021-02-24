using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public class TransferState
    {
        private static readonly List<string> _states;
        static TransferState()
        {
            _states = new List<string>
            {
                "pending",
                "completed",
                "cancelled"
            };
        }

        public static bool In(string value)
        {
            return _states.Contains(value);
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
                Orders = _context.Orders.ToList(),
                Procurements = _context.Procurements.ToList()
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
                order.State = state;

                _context.Orders.Update(order);
                _context.SaveChanges();
                return order;
            }
            catch (Exception e)
            {
                // TODO: add exception handling to be able to create error messages
                return null;
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
                procurement.State = state;

                _context.Procurements.Update(procurement);
                _context.SaveChanges();
                return procurement;
            }
            catch (Exception e)
            {
                // TODO: propagate custom exceptions instead of returning null
                return null;
            }
        }

        private void ValidateState(string state)
        {
            if (!TransferState.In(state))
            {
                // TODO: create custom exception for propagation
                throw new Exception();
            }
        }
    }
}