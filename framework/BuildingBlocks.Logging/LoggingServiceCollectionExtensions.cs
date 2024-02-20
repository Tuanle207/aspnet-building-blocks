namespace BuildingBlocks.Logging
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

            return services;
        }
    }
}
