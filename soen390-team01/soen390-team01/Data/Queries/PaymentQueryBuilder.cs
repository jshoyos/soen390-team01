using soen390_team01.Models;

namespace soen390_team01.Data.Queries
{
    public static class PaymentQueryBuilder
    {
        public static string FilterPayment(Filters filters, string condition)
        {
            return $"Select * From public.payment where {filters.GetConditionsString()} {condition}";
        }
    }
}