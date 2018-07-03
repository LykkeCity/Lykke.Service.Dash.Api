using System;

namespace Lykke.Service.Dash.Api.Core.Domain
{
    public interface IBuild
    {
        Guid OperationId { get; }
        string TransactionContext { get; }
    }
}
