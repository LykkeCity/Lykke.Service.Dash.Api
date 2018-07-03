using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.Dash.Api.Core.Domain;

namespace Lykke.Service.Dash.Api.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    internal class BalanceEntity : AzureTableEntity, IBalance
    {
        public string Address
        {
            get => RowKey;
        }
    }
}
