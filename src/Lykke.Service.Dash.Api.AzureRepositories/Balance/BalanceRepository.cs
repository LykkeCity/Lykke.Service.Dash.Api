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
    public class BalanceRepository : IBalanceRepository
    {
        private INoSQLTableStorage<BalanceEntity> _table;
        private static string GetPartitionKey(string address) => address.CalculateHexHash32(3);
        private static string GetRowKey(string address) => address;

        public BalanceRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory)
        {
            _table = AzureTableStorage<BalanceEntity>.Create(connectionStringManager, "Balances", logFactory);
        }

        public async Task<(IEnumerable<IBalance> Entities, string ContinuationToken)> GetAsync(int take, string continuation)
        {
            return await _table.GetDataWithContinuationTokenAsync(take, continuation);
        }

        public async Task<IBalance> GetAsync(string address)
        {
            return await _table.GetDataAsync(GetPartitionKey(address), GetRowKey(address));
        }

        public async Task AddAsync(string address)
        {
            await _table.InsertOrReplaceAsync(new BalanceEntity
            {
                PartitionKey = GetPartitionKey(address),
                RowKey = GetRowKey(address)
            });
        }

        public async Task DeleteAsync(string address)
        {
            await _table.DeleteIfExistAsync(GetPartitionKey(address), GetRowKey(address));
        }
    }
}
