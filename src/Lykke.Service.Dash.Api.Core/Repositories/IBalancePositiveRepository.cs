﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Dash.Api.Core.Domain;

namespace Lykke.Service.Dash.Api.Core.Repositories
{
    public interface IBalancePositiveRepository
    {
        Task SaveAsync(string address, decimal amount, long block);
        Task DeleteAsync(string address);
        Task<IEnumerable<IBalancePositive>> GetAllAsync();
        Task<IBalancePositive> GetAsync(string address);
        Task<(IEnumerable<IBalancePositive> Entities, string ContinuationToken)> GetAsync(int take, string continuation);
    }
}
