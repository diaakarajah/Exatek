using Exatek.Domain.Dtos;
using Exatek.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Interfaces
{
    public interface ICustomerTokenRepository : IGenericRepository<CustomerToken>
    {
        Task<CustomerToken> GetValidTokenAsync(int customerId, string channel, string token = null);
        Task RevokeTokensByCustomerAndChannelAsync(int customerId, string channel);
    }
}
