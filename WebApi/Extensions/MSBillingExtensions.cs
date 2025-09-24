using Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions
{
    public static class MSBillingExtensions
    {
        public static void AddMSBilling(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MSBillingSettings>(options => configuration.GetSection(nameof(MSBillingSettings)).Bind(options));
            services.Configure<LogSettings>(options => configuration.GetSection(nameof(LogSettings)).Bind(options));
            services.Configure<HttpEndPoints>(options => configuration.GetSection(nameof(HttpEndPoints)).Bind(options));
        }
    }
}