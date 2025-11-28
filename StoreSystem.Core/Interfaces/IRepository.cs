using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StoreSystem.Core.common;

namespace StoreSystem.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();
        Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize);
        Task SaveAsync();
        Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    string? includeProperties = null);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<int> AddAsync(T Entity);
        void DeleteAsync(T Entity);
        Task<bool> UpdateAsync(T Entity);
    }
}