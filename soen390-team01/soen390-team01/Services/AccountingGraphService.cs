﻿using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace soen390_team01.Services
{
    public static class AccountingGraphService
    {
        /// <summary>
        /// This method will generate a graph with payments
        /// </summary>
        /// <param name="payments"></param>
        /// <returns>GraphData</returns>
        public static GraphData CreateGraphData(this List<Payment> payments)
        {
            var monthAmounts = new Dictionary<string, decimal>();

            for (int i = 1; i <= 12; i++)
            {
                monthAmounts.Add(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i), 0);
            }

            foreach (var payment in payments)
            {
                monthAmounts[DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(payment.Updated.Month)] += payment.Amount;
            }

            return new GraphData(monthAmounts.Keys.ToList(), monthAmounts.Values.ToList());
        }
    }

    public class GraphData
    {
        public List<string> Labels { get; set; }
        public List<decimal> Values { get; set; }
        public GraphData(List<string> labels, List<decimal> values)
        {
            Labels = labels;
            Values = values;
        }
    }
}
