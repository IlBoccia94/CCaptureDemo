using Application.Common.Interfaces;
using Application.DocumentProcessing.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Processing;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("Connection string 'PostgreSql' was not found.");

        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.Configure<GoogleCloudOptions>(configuration.GetSection(GoogleCloudOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<DocumentWorkerService>();

        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IImagePreparationService, ImagePreparationService>();
        services.AddScoped<IImageCropper, ImageCropper>();
        services.AddScoped<IOverlayRenderer, OverlayRenderer>();
        services.AddScoped<IGoogleAccessTokenProvider, GoogleAccessTokenProvider>();
        services.AddHttpClient<IVertexDetectionService, VertexDetectionService>();
        services.AddHttpClient<IDocumentAiExtractionService, DocumentAiExtractionService>();

        return services;
    }
}
