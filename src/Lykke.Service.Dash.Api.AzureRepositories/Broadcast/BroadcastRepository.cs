﻿using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common;
using Lykke.SettingsReader;
using Lykke.Service.Dash.Api.Core.Domain;
using Lykke.Service.Dash.Api.Core.Repositories;
using Lykke.Common.Log;

namespace Lykke.Service.Dash.Api.AzureRepositories
{
    public class BroadcastRepository : IBroadcastRepository
    {
        private INoSQLTableStorage<BroadcastEntity> _table;
        private static string GetPartitionKey(Guid operationId) => operationId.ToString().CalculateHexHash32(3);
        private static string GetRowKey(Guid operationId) => operationId.ToString();

        public BroadcastRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory)
        {
            _table = AzureTableStorage<BroadcastEntity>.Create(connectionStringManager, "Broadcasts", logFactory);
        }

        public async Task<IBroadcast> GetAsync(Guid operationId)
        {
            return await _table.GetDataAsync(GetPartitionKey(operationId), GetRowKey(operationId));
        }

        public async Task AddAsync(Guid operationId, string hash, long block)
        {
            await _table.InsertOrReplaceAsync(new BroadcastEntity
            {
                PartitionKey = GetPartitionKey(operationId),
                RowKey = GetRowKey(operationId),
                BroadcastedUtc = DateTime.UtcNow,
                State = BroadcastState.Broadcasted,
                Hash = hash,
                Block = block
            });
        }

        public async Task SaveAsCompletedAsync(Guid operationId, decimal amount, decimal fee, long block)
        {
            await _table.ReplaceAsync(GetPartitionKey(operationId), GetRowKey(operationId), x =>
            {
                x.State = BroadcastState.Completed;
                x.CompletedUtc = DateTime.UtcNow;
                x.Amount = amount;
                x.Fee = fee;
                x.Block = block;

                return x;
            });
        }

        public async Task DeleteAsync(Guid operationId)
        {
            await _table.DeleteIfExistAsync(GetPartitionKey(operationId), GetRowKey(operationId));
        }
    }
}
