using Application.DocumentProcessing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDocumentProcessingOrchestrator, Services.DocumentProcessingOrchestrator>();
        return services;
    }
}
