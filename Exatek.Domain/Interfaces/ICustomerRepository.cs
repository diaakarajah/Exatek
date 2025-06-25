using Exatek.Domain.Entity;
using Exatek.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer> GetByIcNumberAsync(string icNumber);
        Task<bool> IcNumberExistsAsync(string icNumber);
    }
}
