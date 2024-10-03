using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection;

public static class serviceContainer
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Add database connectivity
        // Add auth
        SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FineName"]!);
        
        // crete Dependency Injection
        services.AddScoped<IProduct, ProductRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register middlewares
        // Global Exception
        // Listen to Only API Gateway: blocks all outside calls
        SharedServiceContainer.UseSharedPolicies(app);
        
        return app;
    }
}