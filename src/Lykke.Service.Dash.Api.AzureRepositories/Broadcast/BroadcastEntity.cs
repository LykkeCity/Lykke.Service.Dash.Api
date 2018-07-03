﻿using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.Dash.Api.Core.Domain;

namespace Lykke.Service.Dash.Api.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    internal class BroadcastEntity : AzureTableEntity, IBroadcast
    {
        public Guid OperationId
        {
            get => Guid.Parse(RowKey);
        }

        public BroadcastState State { get; set; }

        public string Hash { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Fee { get; set; }

        public long Block { get; set; }

        public string Error { get; set; }

        public DateTime? BroadcastedUtc { get; set; }

        public DateTime? CompletedUtc { get; set; }

        public DateTime? FailedUtc { get; set; }
    }
}
