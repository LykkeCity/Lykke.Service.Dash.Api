﻿using System;

namespace Lykke.Service.Dash.Api.Core.Domain
{
    public interface IBroadcast
    {
        Guid OperationId { get; }
        BroadcastState State { get; }
        string Hash { get; }
        decimal? Amount { get; }
        decimal? Fee { get; }
        long Block { get; }
        string Error { get; }
        DateTime? BroadcastedUtc { get; }
        DateTime? CompletedUtc { get; }
        DateTime? FailedUtc { get; }
    }
}
