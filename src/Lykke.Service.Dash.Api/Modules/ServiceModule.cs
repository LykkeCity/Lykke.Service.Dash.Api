using Autofac;
using Lykke.SettingsReader;
using Lykke.Service.Dash.Api.Core.Services;
using Lykke.Service.Dash.Api.Core.Repositories;
using Lykke.Service.Dash.Api.Services;
using Lykke.Service.Dash.Api.AzureRepositories;
using Lykke.Service.Dash.Api.Settings;

namespace Lykke.Service.Dash.Api.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public ServiceModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connectionStringManager = _settings.ConnectionString(x => x.DashApiService.Db.DataConnString);

            builder.RegisterType<BroadcastRepository>()
                .As<IBroadcastRepository>()
                .WithParameter(TypedParameter.From(connectionStringManager))
                .SingleInstance();

            builder.RegisterType<BroadcastInProgressRepository>()
                .As<IBroadcastInProgressRepository>()
                .WithParameter(TypedParameter.From(connectionStringManager))
                .SingleInstance();

            builder.RegisterType<BalanceRepository>()
                .As<IBalanceRepository>()
                .WithParameter(TypedParameter.From(connectionStringManager))
                .SingleInstance();

            builder.RegisterType<BalancePositiveRepository>()
                .As<IBalancePositiveRepository>()
                .WithParameter(TypedParameter.From(connectionStringManager))
                .SingleInstance();

            builder.RegisterType<BuildRepository>()
                .As<IBuildRepository>()
                .WithParameter(TypedParameter.From(connectionStringManager))
                .SingleInstance();

            builder.RegisterType<DashInsightClient>()
                .As<IDashInsightClient>()
                .WithParameter("url", _settings.CurrentValue.DashApiService.InsightApiUrl)
                .SingleInstance();

            builder.RegisterType<DashService>()
                .As<IDashService>()
                .WithParameter("network", _settings.CurrentValue.DashApiService.Network)
                .WithParameter("fee", _settings.CurrentValue.DashApiService.Fee)
                .WithParameter("minConfirmations", _settings.CurrentValue.DashApiService.MinConfirmations)
                .SingleInstance();
        }
    }
}
