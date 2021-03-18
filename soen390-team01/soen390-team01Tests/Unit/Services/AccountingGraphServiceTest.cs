using Moq;
using NUnit.Framework;
using soen390_team01.Data.Entities;
using soen390_team01.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace soen390_team01Tests.Unit.Services
{
    class AccountingGraphServiceTest
    {
        

        [Test]
        public void CreateGraphDataTest() 
        {
            List<Payment> payments = new();
            payments.Add(new Payment { Amount = 1, Updated = new DateTime(2021, 1, 1) });

            var monthAmounts = new Dictionary<string, decimal>();
            

            for (int i = 1; i <= 12; i++)
            {
                monthAmounts.Add(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i), 0);
            }

            foreach (var payment in payments)
            {
                monthAmounts[DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(payment.Updated.Month)] += payment.Amount;
            }
            var test = new GraphData(monthAmounts.Keys.ToList(), monthAmounts.Values.ToList());



            Assert.AreEqual(AccountingGraphService.CreateGraphData(payments).Values.Count, test.Values.Count);
            Assert.AreEqual(AccountingGraphService.CreateGraphData(payments).Labels, test.Labels);
            Assert.AreEqual(AccountingGraphService.CreateGraphData(payments).Values, test.Values);
            


        }

    }
}
