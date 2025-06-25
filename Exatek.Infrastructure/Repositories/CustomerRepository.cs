using Exatek.Domain.Entity;
using Exatek.Domain.Interfaces;
using Exatek.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(CustomerDbContext context) : base(context)
        {
        }

        public async Task<Customer> GetByIcNumberAsync(string icNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.ICNumber == icNumber);
        }

        public async Task<bool> IcNumberExistsAsync(string icNumber)
        {
            return await _dbSet.AnyAsync(c => c.ICNumber == icNumber);
        }
    }
}
