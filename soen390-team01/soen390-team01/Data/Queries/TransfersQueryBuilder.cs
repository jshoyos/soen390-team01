using soen390_team01.Models;
using System;

namespace soen390_team01.Data.Queries
{
    public static class TransfersQueryBuilder
    {
        public static string FilterProcurement(Filters filters)
        {
            string temp = filters.GetConditionsString() == ""? filters.GetConditionsString() : "and " + filters.GetConditionsString();
            string query = $"Select procurement.*, vendor.name " +
                   $"From public.procurement, public.vendor where vendor.vendor_id = procurement.vendor_id {temp}";
            query = query.Replace("procurement.vendor ", "vendor.name");
            return query;
        }
        public static string FilterOrder(Filters filters)
        {
            string query = $"Select * From public.{filters.Table} where {filters.GetConditionsString()}";
            query = query.Replace("order.", "public.order.");
            return query;
        }
    }
}
