using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Core.Interfaces
{
    public interface IUniteOfWork : IAsyncDisposable
    {
        Task<int> CompleteAsync();

        Task BeginTransaction();
        Task Commit();
        Task Rollback();
        
    }
}