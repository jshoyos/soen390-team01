using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using soen390_team01.Data.Entities;

namespace soen390_team01.Services
{
    public interface IProductionReportGenerator
    {
        public void Generate(Production prod, string quality);
    }
}
