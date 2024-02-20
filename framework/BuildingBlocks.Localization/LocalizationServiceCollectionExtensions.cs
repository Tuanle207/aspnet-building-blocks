namespace BuildingBlocks.Localization
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Localization;

    public static class LocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IStringLocalizerFactory, JsonResourceManagerStringLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.TryAddTransient(typeof(IStringLocalizer), typeof(StringLocalizer));

            return services;
        }
    }
}
