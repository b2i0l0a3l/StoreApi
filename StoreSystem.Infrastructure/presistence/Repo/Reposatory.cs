using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StoreSystem.Core.common;
using StoreSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

using StoreSystem.Infrastructure.Persistence;

namespace StoreSystem.Infrastructure.Persistence.Repo
{
    public class Reposatory<T> : IRepository<T> where T :class, IEntity
    {
        
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Reposatory(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize)
        {

            var TotoalItems = await _dbSet.CountAsync();
            var items = await _dbSet
            .Skip((pageNumber - 1) * pageSize).
            Take(pageSize).ToListAsync();
            
            return new PagedResult<T>
            {
                Items = items,
                TotalItems = TotoalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
                }

        public async Task<int> AddAsync(T Entity)
        {
            await _dbSet.AddAsync(Entity);
            return Entity.Id;
        }

        public void DeleteAsync(T Entity)
        {
            
            _dbSet.Remove(Entity);
        }

        public async Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize,Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null)
        {
            var query = _dbSet.AsNoTracking(); 
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrWhiteSpace(includeProperties))
    {
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
    }
            var TotoalItems = await query.CountAsync();

            if(orderBy != null)
            {
                query = orderBy(query);
            }


            var items = await query
            .Skip((pageNumber - 1) * pageSize).
            Take(pageSize).ToListAsync();
            
            return new PagedResult<T>
            {
                Items = items,
                TotalItems = TotoalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.FirstOrDefaultAsync(predicate);


        public async Task<bool> UpdateAsync(T Entity)
        {
            var existing = await _dbSet.FindAsync(Entity.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(Entity);
            return true;
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();
    }
}