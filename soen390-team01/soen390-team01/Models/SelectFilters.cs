#region Header

// Author: Tommy Andrews
// File: SelectFilters.cs
// Project: soen390-team01
// Created: 02/24/2021
// 

#endregion

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace soen390_team01.Models
{
    public class SelectFilters : Dictionary<string, SelectList>
    {
        public SelectFilters(string type)
        {
            Type = type;
        }

        public string Type { get; set; }
    }
}