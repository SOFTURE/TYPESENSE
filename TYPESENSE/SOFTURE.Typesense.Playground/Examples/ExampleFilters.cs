using System.Text.Json.Serialization;
using SOFTURE.Typesense.Abstractions.Attributes;
using SOFTURE.Typesense.Abstractions.Enums;
using SOFTURE.Typesense.Abstractions.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleFilters() : FilterBase(collection: "example")
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("identifier")]
    public string? Identifier { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }

    [JsonPropertyName("voivodeship_id")]
    public int? VoivodeshipId { get; set; }

    [FilterOperator(FilterOperator.NotEquals)]
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [FilterOperator(FilterOperator.LessThan)]
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [FilterOperator(FilterOperator.GreaterThan)]
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    [FilterOperator(FilterOperator.LessThanOrEquals)]
    [JsonPropertyName("max_value")]
    public double? MaxValue { get; set; }

    [FilterOperator(FilterOperator.GreaterThanOrEquals)]
    [JsonPropertyName("min_value")]
    public double? MinValue { get; set; }

    [FilterOperator(FilterOperator.Range)]
    [JsonPropertyName("timestamp")]
    public (long From, long To)? Timestamp { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [FilterOperator(FilterOperator.NotEquals)]
    [JsonPropertyName("excluded_ids")]
    public string[]? ExcludedIds { get; set; }

    [FilterOperator(FilterOperator.ExactEquals)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [FilterOperator(FilterOperator.In)]
    [JsonPropertyName("categories")]
    public string[]? Categories { get; set; }

    [FilterOperator(FilterOperator.NotIn)]
    [JsonPropertyName("excluded_categories")]
    public string[]? ExcludedCategories { get; set; }
}