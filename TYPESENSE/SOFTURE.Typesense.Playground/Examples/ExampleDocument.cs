using System.Text.Json.Serialization;
using SOFTURE.Typesense.Abstractions.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleDocument() : DocumentBase(collection: "example")
{
    public override required string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("identifier")]
    public string? Identifier { get; set; }
    
    [JsonPropertyName("city")]
    public string? City { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("voivodeship_id")]
    public int VoivodeshipId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("max_value")]
    public double MaxValue { get; set; }

    [JsonPropertyName("min_value")]
    public double MinValue { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("excluded_ids")]
    public string[]? ExcludedIds { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Status: {Status}, Price: {Price}, Quantity: {Quantity}, Tags: [{string.Join(", ", Tags ?? [])}]";
    }
}