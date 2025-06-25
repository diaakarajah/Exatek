using Exatek.Domain.Interfaces;
using Exatek.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CustomerDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(CustomerDbContext context)
        {
            _context = context;
            Customers = new CustomerRepository(_context);
            CustomerTokens = new CustomerTokenRepository(_context);
            CustomerPins= new CustomerPinRepository(_context);
        }

        public ICustomerRepository Customers { get; private set; }
        public ICustomerTokenRepository CustomerTokens { get; private set; }
        public ICustomerPinRepository CustomerPins { get; private set; }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction?.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction?.RollbackAsync();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
