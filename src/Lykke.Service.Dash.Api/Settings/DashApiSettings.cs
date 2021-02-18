using System;
using System.Collections.Generic;
using Lykke.Service.Dash.Api.Core.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Dash.Api.Settings
{
    public class DashApiSettings
    {
        public DbSettings Db { get; set; }
        public string Network { get; set; }
        public string InsightApiUrl { get; set; }
        public decimal Fee { get; set; }
        public int MinConfirmations { get; set; }
        [Optional]
        public IReadOnlyList<Guid> OperationIdsToRebuild { get; set; }
    }
}
