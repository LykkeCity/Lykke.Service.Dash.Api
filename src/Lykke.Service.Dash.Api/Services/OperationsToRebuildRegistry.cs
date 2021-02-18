using System;
using System.Collections.Generic;
using MoreLinq;

namespace Lykke.Service.Dash.Api.Services
{
    public class OperationsToRebuildRegistry
    {
        private readonly ISet<Guid> _operationIdsToRebuild;

        public OperationsToRebuildRegistry(IReadOnlyList<Guid> operationIdsToRebuild)
        {
            _operationIdsToRebuild = operationIdsToRebuild.ToHashSet();
        }

        public bool HasToRebuild(Guid operationId)
        {
            return _operationIdsToRebuild.Contains(operationId);
        }
    }
}
