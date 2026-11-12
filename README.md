# SOFTURE.Typesense

Typesense wrapper for .NET with support for strongly typed models, filters, and queries.

## Installation

```bash
dotnet add package SOFTURE.Typesense
dotnet add package SOFTURE.Typesense.Abstractions
```

## Quick Start

### 1. Define Document Model

```csharp
public sealed class ProductDocument : DocumentBase
{
    public ProductDocument() : base(collection: "products") { }

    public override required string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
}
```

### 2. Define Query Model

```csharp
public sealed class ProductQuery : QueryBase
{
    public ProductQuery() : base(collection: "products") { }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
```

### 3. Define Filters with Operators

```csharp
public sealed class ProductFilters : FilterBase
{
    public ProductFilters() : base(collection: "products") { }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }

    [FilterOperator(FilterOperator.LessThan)]
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [FilterOperator(FilterOperator.GreaterThan)]
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    [FilterOperator(FilterOperator.Range)]
    [JsonPropertyName("created_at")]
    public (long From, long To)? CreatedAt { get; set; }

    [FilterOperator(FilterOperator.NotEquals)]
    [JsonPropertyName("excluded_ids")]
    public string[]? ExcludedIds { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
}
```

### 4. Define Sorting

```csharp
public sealed class ProductSort : SortBase
{
    public ProductSort() : base(collection: "products") { }

    public ProductSort WithName(SortDirection direction)
    {
        Add("name", direction);
        return this;
    }

    public ProductSort WithPrice(SortDirection direction)
    {
        Add("price", direction);
        return this;
    }
}
```

### 5. Configure Collection

```csharp
public sealed class ProductConfig : ICollectionConfiguration
{
    public ProductConfig()
    {
        Configurations.ConfigureCollection<ProductDocument, ProductQuery, ProductFilters>(
            collectionName: "products",
            fields:
            [
                new Field("name", FieldType.String, facet: false, optional: false, index: true, sort: true),
                new Field("price", FieldType.Float, facet: false, optional: false, index: true, sort: true),
                new Field("is_active", FieldType.Bool, facet: true),
                new Field("tags", FieldType.StringArray, facet: true, optional: true)
            ],
            defaultSortingField: "name"
        );
    }

    public List<CollectionConfiguration> Configurations { get; } = [];
}
```

### 6. Usage

```csharp
await documentClient.ImportDocuments(products, batchSize: 100);

var query = new ProductQuery { Name = "iPhone" };

var filters = new ProductFilters
{
    IsActive = true,
    Price = 1000,
    Tags = ["electronics", "smartphones"]
};

var sort = new ProductSort()
    .WithPrice(SortDirection.Asc)
    .WithName(SortDirection.Desc);

var result = await documentClient.Search<ProductDocument, ProductQuery, ProductFilters, ProductSort>(
    query: query,
    filters: filters,
    sortBy: sort
);
```

## Available Filter Operators

- **Default** - Equals (`=`)
- `FilterOperator.NotEquals` - Not equals (`:!=`)
- `FilterOperator.LessThan` - Less than (`:<`)
- `FilterOperator.LessThanOrEquals` - Less than or equals (`:<=`)
- `FilterOperator.GreaterThan` - Greater than (`:>`)
- `FilterOperator.GreaterThanOrEquals` - Greater than or equals (`:>=`)
- `FilterOperator.Range` - Range (`:[]`) - requires tuple `(From, To)`

## Field Configuration Parameters

- `facet` (default: false) - Fields with `true` can be filtered and grouped
- `index` (default: true) - Set to `false` for fields that should not be searchable
- `sort` (default: false) - Set to `true` for fields that can be sorted
- `optional` (default: false) - Whether the field is optional

## Supported Frameworks

| Target | Typesense client | CSharpFunctionalExtensions |
|--------|------------------|----------------------------|
| `net6.0`, `net8.0` | 7.36.0 | 2.42.5 |
| `net10.0` | 9.2.0 | 3.7.0 |

On `net8.0` and `net10.0` the document `Id` is a `required` member: declare it as
`public override required string Id { get; set; }`, and mark a constructor that sets it with
`[SetsRequiredMembers]`.

The integration tests run against Typesense servers 0.25.1, 28.0 and 30.2. `FilterOperator.NotIn` needs a
server 28.0 or newer; older servers return no hits for it.

## Testing

```bash
cd TYPESENSE
dotnet test
```

The integration tests are skipped unless a Typesense server is configured:

```bash
docker run -d -p 8108:8108 -e TYPESENSE_DATA_DIR=/tmp -e TYPESENSE_API_KEY=xyz typesense/typesense:28.0
TYPESENSE_TEST_HOST=localhost TYPESENSE_TEST_PORT=8108 TYPESENSE_TEST_API_KEY=xyz dotnet test
```

## Releasing

The single workflow [`release.yml`](.github/workflows/release.yml) releases one package per run to nuget.org
and creates a GitHub Release with the `.nupkg` attached.

| Short name | Package |
|------------|---------|
| `typesense` | `SOFTURE.Typesense` |
| `typesense-abstractions` | `SOFTURE.Typesense.Abstractions` |

A release starts from a tag `<package>@<version>` (e.g. `typesense@0.2.0`) created by hand; the workflow never
creates tags. In GitHub: Releases → **Draft a new release** → **Choose a tag** → type `typesense@0.2.0` →
**Create new tag on publish**, target `master` → **Publish release**. Pushing the tag from a terminal works the
same way. `SOFTURE.Typesense` depends on `SOFTURE.Typesense.Abstractions` of the same version, so release both
with one version.

Before publishing, the workflow checks that the commit is on `master`, builds for `net6.0`, `net8.0` and
`net10.0`, runs the unit tests, runs the integration tests against Typesense 0.25.1, 28.0 and 30.2, blocks on
known vulnerabilities on `net10.0` (older targets only warn) and checks the package contents. **Run workflow**
on a branch is a dry run.

Publishing goes through [nuget.org trusted publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing):
the policy (Package Owner `SOFTURE`, Repository `SOFTURE/TYPESENSE`, Workflow File `release.yml`) and the
organization secret `NUGET_USER` (the nuget.org profile name of the policy owner). Keep the workflow file name.

## License

See [LICENSE](LICENSE) file for details.