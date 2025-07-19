using Microsoft.Extensions.DependencyInjection;
using OpenApiToPostman.Core.Managers;
using OpenApiToPostman.Core.Services;
using OpenApiToPostman.Core.Services.Interfaces;

namespace OpenApiToPostman.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register your services here
        services.AddSingleton<IDirectoryService, DirectoryService>();
        services.AddScoped<IOpenApiService, OpenApiService>();
        services.AddScoped<IOpenApiToPostmanTranslationService, OpenApiToPostmanTranslationService>();
        services.AddScoped<IPostmanService, PostmanService>();


        // Register the manager class
        services.AddTransient<OpenApiToPostmanManager>();

        return services;
    }
}