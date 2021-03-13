using soen390_team01.Models;
using System;

namespace soen390_team01.Data.Queries
{
    public static class TransfersQueryBuilder
    {
        public static string FilterProcurement(Filters filters)
        {
            string query = $"Select procurement.procurement_id, procurement.item_id, procurement.payment_id, procurement.item_quantity, procurement.state, procurement.type, procurement.vendor_id, vendor.name " +
                   $"From public.procurement, public.vendor where vendor.vendor_id = procurement.vendor_id and {filters.GetConditionsString()}";
            query = query.Replace("procurement.vendor ", "vendor.name");
            return query;
        }
        public static string FilterOrder(Filters filters)
        {
            return $"Select * From public.{filters.Table} where {filters.GetConditionsString()}";
        }
    }
}
