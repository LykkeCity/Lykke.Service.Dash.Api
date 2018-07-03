using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Sdk;
using Lykke.Service.Dash.Api.Settings;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Logs;

namespace Lykke.Service.Dash.Api
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "Dash.Api";
                options.Logs = logs =>
                {
                    logs.AzureTableName = "DashApiLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.DashApiService.Db.LogsConnString;

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
