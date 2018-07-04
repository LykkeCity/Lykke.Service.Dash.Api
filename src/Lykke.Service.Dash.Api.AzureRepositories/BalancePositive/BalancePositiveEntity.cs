using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.Dash.Api.Core.Domain;

namespace Lykke.Service.Dash.Api.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    internal class BalancePositiveEntity : AzureTableEntity, IBalancePositive
    {
        public string Address
        {
            get => RowKey;
        }

        public decimal Amount { get; set; }

        public long Block { get; set; }
    }
}
