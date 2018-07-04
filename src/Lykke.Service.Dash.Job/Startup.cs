using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Sdk;
using Lykke.Service.Dash.Job.Settings;
using Lykke.Logs.Loggers.LykkeSlack;

namespace Lykke.Service.Dash.Job
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "Dash.Job";
                options.Logs = logs =>
                {
                    logs.AzureTableName = "DashJobLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.DashJob.Db.LogsConnString;

                    logs.Extended = extendedLogs =>
                    {
                        extendedLogs.AddAdditionalSlackChannel("BlockChainIntegration", channelOptions =>
                        {
                            channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                        });
                        extendedLogs.AddAdditionalSlackChannel("BlockChainIntegrationImportantMessages", channelOptions =>
                        {
                            channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                        });
                    };
                };

                options.Swagger = swagger =>
                {
                    swagger.DescribeAllEnumsAsStrings();
                    swagger.DescribeStringEnumsInCamelCase();
                };
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();
        }
    }
}
