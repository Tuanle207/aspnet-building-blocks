namespace BuildingBlock.DemoWebApi.ServiceRegister
{
    using BuildingBlocks.Localization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class LocalizationRegister
    {
        public static IServiceCollection AddLocalization(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<LocalizationOptions>(configuration.GetSection("Localization"));
            services.AddJsonLocalization(configuration);

            return services;
        }

        public static IApplicationBuilder UseAppRequestLocalization(this IApplicationBuilder app)
        {
            var localizationOptions = app.ApplicationServices.GetRequiredService<IOptions<LocalizationOptions>>();
            var supportedCultures = localizationOptions.Value.SupportedCultures.Select(culture => new CultureInfo(culture)).ToList();
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(localizationOptions.Value.DefaultCulture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                ApplyCurrentCultureToResponseHeaders = true,
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider()
                }
            };

            app.UseRequestLocalization(options);

            return app;
        }   
    }
}
