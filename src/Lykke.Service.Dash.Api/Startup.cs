using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Sdk;
using Lykke.Service.Dash.Api.Settings;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Service.BlockchainApi.Contract;
using Microsoft.AspNetCore.Hosting;

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

                options.SwaggerOptions = new LykkeSwaggerOptions
                {
                    ApiTitle = "Dash.Api"
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLykkeMiddleware(ex => BlockchainErrorResponse.FromUnknownError(ex.Message), true);

            app.UseMvc();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = "swagger/ui";
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

        }
    }
}
