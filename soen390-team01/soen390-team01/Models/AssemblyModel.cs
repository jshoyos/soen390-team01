using System;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using soen390_team01.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using soen390_team01.Data.Exceptions;
using Npgsql;
using soen390_team01.Services;
using soen390_team01.Models;
using soen390_team01.Data.Queries;
using System.Globalization;

namespace soen390_team01.Models
{
    public class AssemblyModel : FilteredModel, IAssemblyService
    {
        private readonly ErpDbContext _context;

        public AssemblyModel(ErpDbContext context)
        {
            _context = context;

        }

    }
}
