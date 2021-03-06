﻿using System.Threading.Tasks;
using System.Collections.Generic;
using AzureStorage;
using AzureStorage.Tables;
using Common;
using Lykke.SettingsReader;
using Lykke.Service.Dash.Api.Core.Domain;
using Lykke.Service.Dash.Api.Core.Repositories;
using Lykke.Common.Log;

namespace Lykke.Service.Dash.Api.AzureRepositories
{
    public class BalancePositiveRepository : IBalancePositiveRepository
    {
        private INoSQLTableStorage<BalancePositiveEntity> _table;
        private static string GetPartitionKey(string address) => address.CalculateHexHash32(3);
        private static string GetRowKey(string address) => address;

        public BalancePositiveRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory)
        {
            _table = AzureTableStorage<BalancePositiveEntity>.Create(connectionStringManager, "BalancesPositive", logFactory);
        }

        public async Task<IEnumerable<IBalancePositive>> GetAllAsync()
        {
            return await _table.GetDataAsync();
        }

        public async Task<IBalancePositive> GetAsync(string address)
        {
            return await _table.GetDataAsync(GetPartitionKey(address), GetRowKey(address));
        }

        public async Task<(IEnumerable<IBalancePositive> Entities, string ContinuationToken)> GetAsync(int take, string continuation)
        {
            return await _table.GetDataWithContinuationTokenAsync(take, continuation);
        }

        public async Task SaveAsync(string address, decimal amount, long block)
        {
            await _table.InsertOrReplaceAsync(new BalancePositiveEntity
            {
                PartitionKey = GetPartitionKey(address),
                RowKey = GetRowKey(address),
                Amount = amount,
                Block = block
            });
        }

        public async Task DeleteAsync(string address)
        {
            await _table.DeleteIfExistAsync(GetPartitionKey(address), GetRowKey(address));
        }
    }
}
