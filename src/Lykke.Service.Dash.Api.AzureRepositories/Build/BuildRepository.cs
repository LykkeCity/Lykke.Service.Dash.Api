﻿using System;
using System.Threading.Tasks;
using Lykke.Service.Dash.Api.Core.Repositories;
using Lykke.Service.Dash.Api.Core.Domain;
using Lykke.SettingsReader;
using AzureStorage;
using AzureStorage.Tables;
using Common;
using Lykke.Common.Log;

namespace Lykke.Service.Dash.Api.AzureRepositories
{
    public class BuildRepository : IBuildRepository
    {
        private INoSQLTableStorage<BuildEntity> _table;
        private static string GetPartitionKey(Guid operationId) => operationId.ToString().CalculateHexHash32(3);
        private static string GetRowKey(Guid operationId) => operationId.ToString();

        public BuildRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory)
        {
            _table = AzureTableStorage<BuildEntity>.Create(connectionStringManager, "Builds", logFactory);
        }

        public async Task<IBuild> GetAsync(Guid operationId)
        {
            return await _table.GetDataAsync(GetPartitionKey(operationId), GetRowKey(operationId));
        }

        public async Task AddAsync(Guid operationId, string transactionContext)
        {
            await _table.InsertOrReplaceAsync(new BuildEntity
            {
                PartitionKey = GetPartitionKey(operationId),
                RowKey = GetRowKey(operationId),
                TransactionContext = transactionContext
            });
        }

        public async Task DeleteAsync(Guid operationId)
        {
            await _table.DeleteIfExistAsync(GetPartitionKey(operationId), GetRowKey(operationId));
        }
    }
}
