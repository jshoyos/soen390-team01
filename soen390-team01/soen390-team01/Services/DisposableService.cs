using System;

namespace soen390_team01.Models
{
    public class DisposableService : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = disposing;
        }
    }
}
