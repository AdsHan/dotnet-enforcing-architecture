using EnforcingArchitecture.API.Application.Services;
using EnforcingArchitecture.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EnforcingArchitecture.API.Configuration;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencyConfiguration(this IServiceCollection services)
    {
        services.AddDbContext<CatalogDbContext>(options => options.UseInMemoryDatabase("CatalogDB"));

        services.AddTransient<ProductPopulateService>();

        return services;
    }
}
