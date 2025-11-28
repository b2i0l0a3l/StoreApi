using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Infrastructure.Persistence
{
    public class UniteOfWork : IUniteOfWork
    {
        private readonly AppDbContext _context;
        private ILogger<UniteOfWork> _Logger;
        private IDbContextTransaction? _transaction;
        public UniteOfWork(AppDbContext context,ILogger<UniteOfWork> Logger)
        {
            _Logger = Logger;
            _context = context;
        }


        public async Task BeginTransaction()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
                _Logger.LogInformation("BeginTransaction");

        }

        public async Task Commit()
        {
            try
            {
               await _context.SaveChangesAsync();
                _transaction?.CommitAsync();
                _Logger.LogInformation("Commit");
            }
            catch
            {

                await Rollback();
                throw;
            }
            finally
            {
              _transaction?.DisposeAsync();
            }

        }

        public async Task<int> CompleteAsync()
        =>
              await _context.SaveChangesAsync();
        


        public async ValueTask DisposeAsync()
        {
             _transaction?.DisposeAsync();
            await _context.DisposeAsync();
            _Logger.LogInformation("Dispose");
        }

        public async Task Rollback()
        {
            _transaction?.RollbackAsync();
            _transaction?.DisposeAsync();
            _Logger.LogError("Rollback");

        }


    }
}