using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
            (context, config) =>
            {
                var env = context.HostingEnvironment;
                config.MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                    .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Warning)
                    .WriteTo.Console();

                if (env.IsDevelopment())
                {
                    config.MinimumLevel.Override("Catalog", Serilog.Events.LogEventLevel.Debug);
                    config.MinimumLevel.Override("Basket", Serilog.Events.LogEventLevel.Debug);
                    config.MinimumLevel.Override("Discount", Serilog.Events.LogEventLevel.Debug);
                    config.MinimumLevel.Override("Ordering", Serilog.Events.LogEventLevel.Debug);
                }

                // Elastic Search
                var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
                if (!string.IsNullOrEmpty(elasticUrl))
                {
                    config.WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(elasticUrl))
                        {
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                            IndexFormat = "Ecommerce-Logs-{0:yyyy.MM.dd}",
                            MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug,
                        });
                }
            };
    }
}
