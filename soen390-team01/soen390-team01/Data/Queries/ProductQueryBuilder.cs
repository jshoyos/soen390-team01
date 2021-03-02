using soen390_team01.Models;

namespace soen390_team01.Data.Queries
{
    public static class ProductQueryBuilder
    {
        public static string FilterProduct(Filters filters)
        {
            return $"Select * From public.{filters.Table} where {filters.GetConditionsString()}";
        }

        public static string GetProduct(string table, long itemId)
        {
            return $"Select * From public.{table} where item_id = {itemId}";
        }
    }
}
