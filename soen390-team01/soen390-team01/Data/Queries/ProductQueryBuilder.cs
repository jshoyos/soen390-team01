#region Header

// Author: Tommy Andrews
// File: ProductQueryBuilder.cs
// Project: soen390-team01
// Created: 02/25/2021
// 

#endregion

using soen390_team01.Models;

namespace soen390_team01.Data.Queries
{
    public class ProductQueryBuilder
    {
        public static string FilterProduct(ProductFilterInput input)
        {
            return $"Select * From public.{input.Type} where {input.Name} = '{input.Value}'";
        }

        public static string GetProduct(string table, long itemId)
        {
            return $"Select * From public.{table} where item_id = {itemId}";
        }
    }
}