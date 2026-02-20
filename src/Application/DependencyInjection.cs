using Application.DocumentProcessing.Interfaces;
using Application.DocumentProcessing.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDocumentProcessingOrchestrator, DocumentProcessingOrchestrator>();
        return services;
    }
}
