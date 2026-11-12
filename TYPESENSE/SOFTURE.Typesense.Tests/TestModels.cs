using System.Text.Json.Serialization;
using SOFTURE.Typesense.Abstractions.Attributes;
using SOFTURE.Typesense.Abstractions.Enums;
using SOFTURE.Typesense.Abstractions.Models;
using Typesense;

namespace SOFTURE.Typesense.Tests;

public sealed class ProductDocument() : DocumentBase(collection: ProductCollection.Name)
{
    public override required string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = [];
}

public sealed class ProductQuery() : QueryBase(collection: ProductCollection.Name)
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }
}

public sealed class ProductFilters() : FilterBase(collection: ProductCollection.Name)
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }

    [FilterOperator(FilterOperator.GreaterThan)]
    [JsonPropertyName("quantity")]
    public int? QuantityAbove { get; set; }

    [FilterOperator(FilterOperator.Range)]
    [JsonPropertyName("timestamp")]
    public (long From, long To)? Timestamp { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [FilterOperator(FilterOperator.NotIn)]
    [JsonPropertyName("name")]
    public string[]? ExcludedNames { get; set; }
}

public sealed class ProductSort() : SortBase(collection: ProductCollection.Name);

public static class ProductCollection
{
    public static readonly string Name = $"softure_typesense_tests_products_net{Environment.Version.Major}";

    public static List<Field> Fields() =>
    [
        new("name", FieldType.String) { Sort = true },
        new("city", FieldType.String) { Facet = true },
        new("is_active", FieldType.Bool),
        new("quantity", FieldType.Int32),
        new("timestamp", FieldType.Int64),
        new("tags", FieldType.StringArray) { Facet = true }
    ];
}
