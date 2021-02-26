#region Header

// Author: Tommy Andrews
// File: DisposableService.cs
// Project: soen390-team01
// Created: 02/25/2021
// 

#endregion

using System;

namespace soen390_team01.Models
{
    public class DisposableService : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }
    }
}