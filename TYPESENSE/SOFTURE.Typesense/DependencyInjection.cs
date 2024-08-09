using Microsoft.Extensions.DependencyInjection;
using SOFTURE.Common.HealthCheck;
using SOFTURE.Settings.Extensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Clients;
using SOFTURE.Typesense.HealthChecks;
using SOFTURE.Typesense.Settings;
using Typesense.Setup;

namespace SOFTURE.Typesense
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTypesense<TSettings, TCollectionConfiguration>(this IServiceCollection services)
            where TSettings : ITypesenseSettings
            where TCollectionConfiguration : class, ICollectionConfiguration
        {
            services.AddSingleton<ICollectionConfiguration, TCollectionConfiguration>();
            
            var settings = services.GetSettings<TSettings, TypesenseSettings>(x => x.Typesense);

            services.AddCommonHealthCheck<TypesenseHealthCheck>();

            services.AddTypesenseClient(config =>
            {
                config.ApiKey = settings.ApiKey;
                config.Nodes = new List<Node>
                {
                    new(settings.Host, settings.Port, "http")
                };
            });

            services.AddScoped<ICollectionClient, CollectionClient>();
            services.AddScoped<IDocumentClient, DocumentClient>();
            
            return services;
        }
    }
}