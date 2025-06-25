using Exatek.Domain.Dtos;
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
    public class CustomerTokenRepository : GenericRepository<CustomerToken>, ICustomerTokenRepository
    {
        public CustomerTokenRepository(CustomerDbContext context) : base(context)
        {
        }

        public async Task<CustomerToken> GetValidTokenAsync(int customerId, string channel, string token = null)
        {
            var query = _dbSet.Where(t => t.CustomerId == customerId
                                        && t.Channel == channel
                                        && !t.IsRevoked
                                        && t.ExpiredAt > DateTime.Now);

            if (!string.IsNullOrEmpty(token))
            {
                query = query.Where(t => t.Token == token);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task RevokeTokensByCustomerAndChannelAsync(int customerId, string channel)
        {
            var tokens = await _dbSet
                .Where(t => t.CustomerId == customerId && t.Channel == channel && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.UpdatedAt = DateTime.Now;
            }
        }
    }
}
