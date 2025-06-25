using Exatek.Domain.Entity;
using Exatek.Domain.Interfaces;
using Exatek.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Infrastructure.Repositories
{
    public class CustomerPinRepository : GenericRepository<CustomerPin>, ICustomerPinRepository
    {
        public CustomerPinRepository(CustomerDbContext context) : base(context)
        {
        }
    }
}
