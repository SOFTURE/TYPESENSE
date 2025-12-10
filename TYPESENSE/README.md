# SOFTURE.Typesense

A .NET client library for Typesense that provides full support for all filtering operators and type-safe query building.

## Features

- ✅ Complete support for all 10 Typesense filtering operators
- ✅ Type-safe query and filter building
- ✅ Attribute-based filter configuration
- ✅ Collection management and document operations
- ✅ Built-in validation and error handling

## Supported Filtering Operators

This library provides complete support for all Typesense filtering operators:

| Operator | Description | FilterOperator Enum | Example Usage |
|----------|-------------|--------------------|--------------| 
| `=` | Exact match | `FilterOperator.ExactEquals` | `country:=USA` |
| `:` | Partially equal to | `FilterOperator.Equals` | `country:New` |
| `!=` | Not equal to | `FilterOperator.NotEquals` | `status:!=inactive` |
| `<` | Less than | `FilterOperator.LessThan` | `price:<100` |
| `>` | Greater than | `FilterOperator.GreaterThan` | `price:>100` |
| `<=` | Less than or equal to | `FilterOperator.LessThanOrEquals` | `price:<=100` |
| `>=` | Greater than or equal to | `FilterOperator.GreaterThanOrEquals` | `price:>=100` |
| `[]` | Is one of | `FilterOperator.In` | `country:[USA, UK, Canada]` |
| `![]` | Is not any of | `FilterOperator.NotIn` | `country:![USA, UK, Canada]` |
| `[..]` | Range | `FilterOperator.Range` | `price:[100..200]` |

## Quick Start

### 1. Define Your Document Model

```csharp
public class Product : DocumentBase
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("category")]
    public string Category { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
}
```

### 2. Create Filter Model with Operators

```csharp
public class ProductFilters : FilterBase
{
    public ProductFilters() : base("products") { }
    
    // Default operator (partially equal)
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    // Exact match
    [FilterOperator(FilterOperator.ExactEquals)]
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    
    // Numeric comparisons
    [FilterOperator(FilterOperator.GreaterThan)]
    [JsonPropertyName("price")]
    public decimal? MinPrice { get; set; }
    
    [FilterOperator(FilterOperator.LessThanOrEquals)]
    [JsonPropertyName("price")]
    public decimal? MaxPrice { get; set; }
    
    // Array operations
    [FilterOperator(FilterOperator.In)]
    [JsonPropertyName("category")]
    public string[]? Categories { get; set; }
    
    [FilterOperator(FilterOperator.NotIn)]
    [JsonPropertyName("status")]
    public string[]? ExcludedStatuses { get; set; }
    
    // Range operations
    [FilterOperator(FilterOperator.Range)]
    [JsonPropertyName("price")]
    public (decimal From, decimal To)? PriceRange { get; set; }
    
    // Boolean equality
    [FilterOperator(FilterOperator.NotEquals)]
    [JsonPropertyName("is_active")]
    public bool? IsNotActive { get; set; }
}
```

### 3. Configure Collection

```csharp
public class ProductConfig : ICollectionConfiguration
{
    public ProductConfig()
    {
        Configurations.ConfigureCollection<Product, ProductQuery, ProductFilters>(
            collectionName: "products",
            fields: [
                new Field("name", FieldType.String, facet: false, index: true),
                new Field("price", FieldType.Float, facet: true, index: true),
                new Field("category", FieldType.String, facet: true, index: true),
                new Field("is_active", FieldType.Bool, facet: true)
            ],
            defaultSortingField: "name"
        );
    }
    
    public List<CollectionConfiguration> Configurations { get; } = [];
}
```

### 4. Usage Examples

```csharp
// Search with filters
var filters = new ProductFilters
{
    Name = "laptop",                    // name:laptop (partial match)
    Category = "electronics",           // category:=electronics (exact match)
    MinPrice = 500,                     // price:>500
    MaxPrice = 2000,                    // price:<=2000
    Categories = ["electronics", "tech"], // category:[electronics,tech]
    ExcludedStatuses = ["discontinued"], // status:![discontinued]
    PriceRange = (100, 1000),          // price:[100..1000]
    IsNotActive = false                 // is_active:!=false
};

var results = await documentClient.SearchAsync<Product>(
    query: "laptop computer",
    filters: filters
);
```

## Advanced Filtering Examples

### Exact vs Partial Matching

```csharp
// Partial matching (default) - finds "New York", "New Jersey", etc.
[JsonPropertyName("location")]
public string? Location { get; set; }  // location:New

// Exact matching - finds only "New York"
[FilterOperator(FilterOperator.ExactEquals)]
[JsonPropertyName("city")]
public string? City { get; set; }  // city:="New York"
```

### Array Operations

```csharp
// Include items with any of these categories
[FilterOperator(FilterOperator.In)]
[JsonPropertyName("tags")]
public string[]? IncludedTags { get; set; }  // tags:[tech,gadget,mobile]

// Exclude items with these categories  
[FilterOperator(FilterOperator.NotIn)]
[JsonPropertyName("tags")]
public string[]? ExcludedTags { get; set; }  // tags:![spam,adult,illegal]
```

### Numeric Range Operations

```csharp
// Single range
[FilterOperator(FilterOperator.Range)]
[JsonPropertyName("price")]
public (decimal Min, decimal Max)? PriceRange { get; set; }  // price:[100..500]

// Multiple constraints
[FilterOperator(FilterOperator.GreaterThanOrEquals)]
[JsonPropertyName("rating")]
public float? MinRating { get; set; }  // rating:>=4.0

[FilterOperator(FilterOperator.LessThan)]
[JsonPropertyName("review_count")]
public int? MaxReviews { get; set; }  // review_count:<1000
```

## Installation

```bash
dotnet add package SOFTURE.Typesense
```

## Configuration

Add Typesense to your service collection:

```csharp
services.AddTypesense(configuration =>
{
    configuration.Nodes = [new Node("localhost", "8108", "http")];
    configuration.ApiKey = "your-api-key";
    configuration.ConnectionTimeoutSeconds = 2;
    configuration.NumRetries = 3;
});
```

## Documentation

For more information about Typesense filtering, visit the [official documentation](https://typesense.org/docs/guide/tips-for-filtering.html#available-operators).