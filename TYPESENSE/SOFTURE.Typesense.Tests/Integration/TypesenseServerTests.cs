using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Abstractions.Models;
using SOFTURE.Typesense.Extensions;
using SOFTURE.Typesense.Interfaces;
using Typesense;
using Xunit;

namespace SOFTURE.Typesense.Tests.Integration;

[Collection(TypesenseServerCollection.Name)]
public sealed class TypesenseServerTests(TypesenseServer server)
{
    private static readonly ProductDocument[] Products =
    [
        new() { Id = "1", Name = "Laptop Pro", City = "Warszawa", IsActive = true, Quantity = 10, Timestamp = 150, Tags = ["red", "electronics"] },
        new() { Id = "2", Name = "Laptop Air", City = "Warszawa", IsActive = true, Quantity = 3, Timestamp = 160, Tags = ["blue", "electronics"] },
        new() { Id = "3", Name = "Desk Lamp", City = "Krakow", IsActive = true, Quantity = 25, Timestamp = 170, Tags = ["red", "home"] },
        new() { Id = "4", Name = "Laptop Old", City = "Warszawa", IsActive = false, Quantity = 40, Timestamp = 120, Tags = ["red"] }
    ];

    [TypesenseFact]
    public async Task CreateCollection_CreatesOnceThenReportsNotModified()
    {
        var provider = server.Create(TypesenseServer.Products());
        await TypesenseServer.DropProducts(provider);
        var collections = provider.GetRequiredService<ICollectionClient>();
        var configuration = provider.GetRequiredService<ICollectionConfiguration>().Configurations.Single();

        var first = await collections.CreateCollection(configuration);
        var second = await collections.CreateCollection(configuration);

        first.Value.Should().Be(CreationState.Created);
        second.Value.Should().Be(CreationState.NotModified);
    }

    [TypesenseFact]
    public async Task CreateCollection_WhenSchemaChanges_ReportsUpdatedThenNotModified()
    {
        var original = server.Create(TypesenseServer.Products());
        await TypesenseServer.DropProducts(original);
        await original.GetRequiredService<ICollectionClient>().CreateCollection(original.GetRequiredService<ICollectionConfiguration>().Configurations.Single());

        var changed = server.Create(TypesenseServer.Products(new Field("price", FieldType.Float) { Optional = true }));
        var collections = changed.GetRequiredService<ICollectionClient>();
        var configuration = changed.GetRequiredService<ICollectionConfiguration>().Configurations.Single();

        (await collections.CreateCollection(configuration)).Value.Should().Be(CreationState.Updated);
        (await collections.CreateCollection(configuration)).Value.Should().Be(CreationState.NotModified);
    }

    [TypesenseFact]
    public async Task MigrateTypesenseCollections_CreatesConfiguredCollection()
    {
        var provider = server.Create(TypesenseServer.Products());
        await TypesenseServer.DropProducts(provider);
        var services = new ServiceCollection();
        services.AddSingleton(provider.GetRequiredService<ICollectionClient>());
        services.AddSingleton(provider.GetRequiredService<ICollectionConfiguration>());

        services.MigrateTypesenseCollections();

        var again = await provider.GetRequiredService<ICollectionClient>().CreateCollection(provider.GetRequiredService<ICollectionConfiguration>().Configurations.Single());
        again.Value.Should().Be(CreationState.NotModified);
    }

    [TypesenseFact]
    public async Task Search_WithQueryFiltersAndSort_ReturnsMatchingDocumentsInOrder()
    {
        var documents = await SeedProducts();

        var result = await documents.Search<ProductDocument, ProductQuery, ProductFilters, ProductSort>(
            new ProductQuery { Name = "laptop" },
            filters: new ProductFilters { City = "Warszawa", IsActive = true, Tags = ["electronics"] },
            sortBy: Sort("quantity", SortDirection.Desc));

        result.IsSuccess.Should().BeTrue(result.IsFailure ? result.Error : string.Empty);
        result.Value.Items.Select(x => x.Id).Should().Equal("1", "2");
        result.Value.Found.Should().Be(2);
        result.Value.Total.Should().Be(4);
        result.Value.Page.Should().Be(1);
        result.Value.Items[0].Name.Should().Be("Laptop Pro");
        result.Value.Items[0].Tags.Should().Equal("red", "electronics");
    }

    [TypesenseFact]
    public async Task Search_WithNumericRangeAndExclusionFilters_ReturnsMatchingDocuments()
    {
        var documents = await SeedProducts();

        var result = await documents.Search<ProductDocument, ProductQuery, ProductFilters, ProductSort>(
            new ProductQuery { Name = "laptop" },
            filters: new ProductFilters { QuantityAbove = 5, Timestamp = (140, 200) },
            typoTolerance: false,
            sortBy: Sort("quantity", SortDirection.Asc));

        result.IsSuccess.Should().BeTrue(result.IsFailure ? result.Error : string.Empty);
        result.Value.Items.Select(x => x.Id).Should().Equal("1");
    }

    [TypesenseFact]
    public async Task Search_WithNotInFilter_ExcludesListedValues()
    {
        if (await server.MajorVersion() < 28)
        {
            return;
        }

        var documents = await SeedProducts();

        var result = await documents.Search<ProductDocument, ProductQuery, ProductFilters, ProductSort>(
            new ProductQuery { Name = "laptop" },
            filters: new ProductFilters { ExcludedNames = ["laptop old", "laptop air"] },
            sortBy: Sort("quantity", SortDirection.Asc));

        result.IsSuccess.Should().BeTrue(result.IsFailure ? result.Error : string.Empty);
        result.Value.Items.Select(x => x.Id).Should().Equal("1");
    }

    [TypesenseFact]
    public async Task Search_PagesThroughResults()
    {
        var documents = await SeedProducts();

        var page = await documents.Search<ProductDocument, ProductQuery, ProductFilters, ProductSort>(
            new ProductQuery { Name = "laptop" },
            page: 2,
            records: 2,
            sortBy: Sort("quantity", SortDirection.Asc));

        page.IsSuccess.Should().BeTrue(page.IsFailure ? page.Error : string.Empty);
        page.Value.Found.Should().Be(3);
        page.Value.Page.Should().Be(2);
        page.Value.Items.Select(x => x.Id).Should().Equal("4");
    }

    [TypesenseFact]
    public async Task UpsertCreateAndDelete_ChangeWhatSearchReturns()
    {
        var documents = await SeedProducts();

        (await documents.UpsertDocument(new ProductDocument { Id = "2", Name = "Laptop Air", City = "Warszawa", IsActive = true, Quantity = 99, Timestamp = 160, Tags = ["blue"] })).IsSuccess.Should().BeTrue();
        (await documents.UpsertDocument(new ProductDocument { Id = "5", Name = "Laptop Mini", City = "Gdansk", IsActive = true, Quantity = 1, Timestamp = 180, Tags = [] })).IsSuccess.Should().BeTrue();
        (await documents.CreateDocument(new ProductDocument { Id = "6", Name = "Laptop Max", City = "Gdansk", IsActive = true, Quantity = 7, Timestamp = 190, Tags = [] })).IsSuccess.Should().BeTrue();
        (await documents.DeleteDocument(Products[0])).IsSuccess.Should().BeTrue();

        var result = await documents.Search<ProductDocument, ProductQuery, ProductFilters, ProductSort>(
            new ProductQuery { Name = "laptop" },
            sortBy: Sort("quantity", SortDirection.Desc));

        result.IsSuccess.Should().BeTrue(result.IsFailure ? result.Error : string.Empty);
        result.Value.Items.Select(x => $"{x.Id}:{x.Quantity}").Should().Equal("2:99", "4:40", "6:7", "5:1");
    }

    [TypesenseFact]
    public async Task CreateDocument_WhenIdAlreadyExists_ReturnsFailure()
    {
        var documents = await SeedProducts();

        var result = await documents.CreateDocument(Products[0]);

        result.IsFailure.Should().BeTrue();
    }

    [TypesenseFact]
    public async Task Search_WhenFilterFieldIsUnknown_ReturnsFailureInsteadOfThrowing()
    {
        var documents = await SeedProducts();

        var result = await documents.Search<ProductDocument, ProductQuery, UnknownFieldFilters, ProductSort>(
            new ProductQuery { Name = "laptop" },
            filters: new UnknownFieldFilters { Missing = "x" });

        result.IsFailure.Should().BeTrue();
    }

    private async Task<IDocumentClient> SeedProducts()
    {
        var provider = server.Create(TypesenseServer.Products());
        await TypesenseServer.DropProducts(provider);
        await provider.GetRequiredService<ICollectionClient>().CreateCollection(provider.GetRequiredService<ICollectionConfiguration>().Configurations.Single());

        var documents = provider.GetRequiredService<IDocumentClient>();
        var import = await documents.ImportDocuments(Products.ToList());

        import.IsSuccess.Should().BeTrue(import.IsFailure ? import.Error : string.Empty);

        return documents;
    }

    private static ProductSort Sort(string field, SortDirection direction)
    {
        var sort = new ProductSort();
        sort.Add(field, direction);
        return sort;
    }

    public sealed class UnknownFieldFilters() : FilterBase(collection: ProductCollection.Name)
    {
        public string? Missing { get; set; }
    }
}
