using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection; // già incluso implicitamente
using Microsoft.Extensions.Configuration; // già incluso implicitamente
using Swashbuckle.AspNetCore.SwaggerGen; // <--- Questo è quello mancante

namespace Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.SectionName));

        return services;
    }
}
