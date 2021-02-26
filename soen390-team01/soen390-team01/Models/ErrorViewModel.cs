#region Header

// Author: Tommy Andrews
// File: ErrorViewModel.cs
// Project: soen390-team01
// Created: 01/24/2021
// 

#endregion

namespace soen390_team01.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}