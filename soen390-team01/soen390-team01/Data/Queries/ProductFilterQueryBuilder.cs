using soen390_team01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Data.Queries
{
    public class ProductFilterQueryBuilder
    {
        public static string FilterProduct(ProductFilterInput input)
        {
            return $"Select * From public.{input.Type} where {input.Name} = '{input.Value}'";
        }
    }
}
