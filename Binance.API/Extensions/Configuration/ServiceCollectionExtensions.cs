using AspNetCoreRateLimit;
using Binance.Domain.Interfaces;
using Binance.Domain.Interfaces.Repositories;
using Binance.Domain.Settings;
using Binance.Infrastructure;
using Binance.Infrastructure.BackgroundServices;
using Binance.Infrastructure.Mediator.Handlers;
using Binance.Infrastructure.Mediator.Requests;
using Binance.Infrastructure.Repositories;
using Binance.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WatchDog;
using WatchDog.src.Enums;

namespace Binance.API.Extensions.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSettings(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.Configure<BinanceSyncSettings>(configurationManager.GetSection(nameof(BinanceSyncSettings)));
        
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(x => AddSwaggerDocumentation(x));



        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<BinanceSyncBackgroundService>();

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ISymbolPriceService, SymbolPriceService>();

        services.AddScoped<ISymbolRepository, SymbolRepository>();
        services.AddScoped<ISymbolPriceRepository, SymbolPriceRepository>();

        return services;
    }

    public static IServiceCollection RegisterMediatR(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<Get24hAvgPriceRequest, decimal>, Get24hAvgPriceHandler>();
        services.AddTransient<IRequestHandler<GetSimpleMovingAverageRequest, decimal>, GetSimpleMovingAverageHandler>();

        return services;
    }

    public static IServiceCollection AddContext(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var connctionString = configurationManager.GetConnectionString("SqlServer");

        services.AddDbContext<BinanceContext>(options =>
                options.UseSqlServer(connctionString, options => options.MigrationsAssembly(typeof(BinanceContext).Assembly.GetName().Name)), 
                ServiceLifetime.Scoped, ServiceLifetime.Scoped);

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    public static IServiceCollection AddWatchDog(this IServiceCollection services)
    {
        services.AddWatchDogServices(settings =>
        {
            settings.IsAutoClear = true;
            settings.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Daily;
        });

        return services;
    }
    private static void AddSwaggerDocumentation(SwaggerGenOptions o)
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }
}
