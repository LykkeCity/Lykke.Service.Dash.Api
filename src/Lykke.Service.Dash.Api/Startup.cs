using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Sdk;
using Lykke.Service.Dash.Api.Settings;
using Lykke.Logs.Loggers.LykkeSlack;

namespace Lykke.Service.Dash.Api
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.Logs = logs =>
                {
                    logs.AzureTableName = "DashApiLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.DashApiService.Db.LogsConnString;

                    logs.Extended = extendedLogs =>
                    {
                        extendedLogs.AddAdditionalSlackChannel("BlockChainIntegration");
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

                options.SwaggerOptions.ApiTitle = "Dash.Api";
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();
        }
    }
}
