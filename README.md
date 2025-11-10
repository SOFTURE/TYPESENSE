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

## License

See [LICENSE](LICENSE) file for details.