using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.Dash.Api.Core.Domain;

namespace Lykke.Service.Dash.Api.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    internal class BuildEntity : AzureTableEntity, IBuild
    {
        public Guid OperationId
        {
            get => Guid.Parse(RowKey);
        }

        public string TransactionContext { get; set; }
    }
}
