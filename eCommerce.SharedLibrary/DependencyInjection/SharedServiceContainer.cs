using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedServices<TContext>
        (this IServiceCollection services, IConfiguration config, string fileName) 
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(option => option.UseSqlServer(
            config.GetConnectionString("eCommerceConnection"), sqlserveroptions => 
                sqlserveroptions.EnableRetryOnFailure()));
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{timestamp:yyyy-MM-dd HH:mm:ss:fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger(); 
            
        services.AddJWTAuthenticationScheme(config);
        
        return services;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        // use global exception
        app.UseMiddleware<GlobalException>();
        
        //register middleware to block all outsider API calls
        //app.UseMiddleware<ListenToOnlyApiGateway>();

        return app;
    }
}