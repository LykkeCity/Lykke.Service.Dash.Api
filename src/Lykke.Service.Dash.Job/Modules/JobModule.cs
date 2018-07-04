using Autofac;
using Lykke.Common.Chaos;
using Lykke.SettingsReader;
using Lykke.Service.Dash.Api.Core.Services;
using Lykke.Service.Dash.Api.Core.Repositories;
using Lykke.Service.Dash.Api.Services;
using Lykke.Service.Dash.Api.AzureRepositories;
using Lykke.Service.Dash.Job.PeriodicalHandlers;
using Lykke.Service.Dash.Job.Settings;
using Lykke.Service.Dash.Job.Services;

namespace Lykke.Service.Dash.Job.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public JobModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connectionStringManager = _settings.ConnectionString(x => x.DashJob.Db.DataConnString);

            builder.RegisterChaosKitty(_settings.CurrentValue.DashJob.ChaosKitty);

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

            builder.RegisterType<DashInsightClient>()
                .As<IDashInsightClient>()
                .WithParameter("url", _settings.CurrentValue.DashJob.InsightApiUrl)
                .SingleInstance();

            builder.RegisterType<PeriodicalService>()
                .As<IPeriodicalService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.DashJob.MinConfirmations))
                .SingleInstance();            

            builder.RegisterType<BalanceHandler>()
                .As<IStartable>()
                .AutoActivate()
                .WithParameter("period", _settings.CurrentValue.DashJob.BalanceCheckerInterval)
                .WithParameter("disableErrorsSending", _settings.CurrentValue.DashJob.DisableErrorsSending)
                .SingleInstance();

            builder.RegisterType<BroadcastHandler>()
                .As<IStartable>()
                .AutoActivate()
                .WithParameter("period", _settings.CurrentValue.DashJob.BroadcastCheckerInterval)
                .WithParameter("disableErrorsSending", _settings.CurrentValue.DashJob.DisableErrorsSending)
                .SingleInstance();
        }
    }
}
