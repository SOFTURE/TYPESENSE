using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SOFTURE.Settings.Extensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Extensions;
using SOFTURE.Typesense.Interfaces;
using SOFTURE.Typesense.Settings;
using SOFTURE.Typesense.ValueObjects;
using Xunit;

namespace SOFTURE.Typesense.Tests.Integration;

public sealed class TypesenseFactAttribute : FactAttribute
{
    public TypesenseFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(TypesenseServer.HostVariable)))
        {
            Skip = $"Set {TypesenseServer.HostVariable} (and optionally {TypesenseServer.PortVariable}, {TypesenseServer.ApiKeyVariable}) to run against a Typesense server.";
        }
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class TypesenseServerCollection : ICollectionFixture<TypesenseServer>
{
    public const string Name = "typesense-server";
}

public sealed class TypesenseServer : IDisposable
{
    public const string HostVariable = "TYPESENSE_TEST_HOST";
    public const string PortVariable = "TYPESENSE_TEST_PORT";
    public const string ApiKeyVariable = "TYPESENSE_TEST_API_KEY";

    private readonly List<ServiceProvider> _providers = [];

    public ServiceProvider Create(List<CollectionConfiguration> configurations)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Typesense:Host"] = Environment.GetEnvironmentVariable(HostVariable) ?? "localhost",
                ["Typesense:Port"] = Environment.GetEnvironmentVariable(PortVariable) ?? "8108",
                ["Typesense:ApiKey"] = Environment.GetEnvironmentVariable(ApiKeyVariable) ?? "xyz"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton(configurations);
        services.ConfigureSettings<TestSettings>(configuration);
        services.AddTypesense<TestSettings, TestCollectionConfiguration>();

        var provider = services.BuildServiceProvider();
        _providers.Add(provider);

        return provider;
    }

    public async Task<int> MajorVersion()
    {
        var host = Environment.GetEnvironmentVariable(HostVariable) ?? "localhost";
        var port = Environment.GetEnvironmentVariable(PortVariable) ?? "8108";
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", Environment.GetEnvironmentVariable(ApiKeyVariable) ?? "xyz");

        var debug = await http.GetFromJsonAsync<JsonElement>($"http://{host}:{port}/debug");
        var version = debug.GetProperty("version").GetString() ?? "0";

        return int.Parse(version.Split('.')[0]);
    }

    public static List<CollectionConfiguration> Products(params global::Typesense.Field[] extraFields)
    {
        var configurations = new List<CollectionConfiguration>();
        var fields = ProductCollection.Fields();
        fields.AddRange(extraFields);

        configurations.ConfigureCollection<ProductDocument, ProductQuery, ProductFilters>(ProductCollection.Name, fields, "quantity");

        return configurations;
    }

    public static async Task DropProducts(IServiceProvider provider)
    {
        await provider.GetRequiredService<ICollectionClient>().DeleteCollection(Collection.Create(ProductCollection.Name).Value);
    }

    public void Dispose()
    {
        foreach (var provider in _providers)
        {
            provider.Dispose();
        }
    }

    public sealed class TestSettings : ITypesenseSettings
    {
        public TypesenseSettings Typesense { get; init; } = null!;
    }

    public sealed class TestCollectionConfiguration(List<CollectionConfiguration> configurations) : ICollectionConfiguration
    {
        public List<CollectionConfiguration> Configurations { get; } = configurations;
    }
}
