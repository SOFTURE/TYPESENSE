using Microsoft.Extensions.DependencyInjection;
using SOFTURE.Typesense.Interfaces;

namespace SOFTURE.Typesense.Extensions;

public static class TypesenseExtensions
{
    public static IServiceCollection MigrateTypesenseCollections(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var collectionClient = serviceProvider.GetRequiredService<ICollectionClient>();
        var collectionConfiguration = serviceProvider.GetRequiredService<ICollectionConfiguration>();

        if (collectionConfiguration.Configurations.Count == 0)
        {
            Console.WriteLine("[TYPESENSE] No collections to migrate");
        }

        foreach (var configuration in collectionConfiguration.Configurations)
        {
            var creationResult = collectionClient.CreateCollection(configuration).ConfigureAwait(true).GetAwaiter().GetResult();

            Console.WriteLine(creationResult.IsSuccess
                ? $"[TYPESENSE] Collection '{configuration.Collection.Name}' - {creationResult.Value}"
                : $"[TYPESENSE] Collection '{configuration.Collection.Name}' creation/update failed: {creationResult.Error}");
        }

        return services;
    }
}