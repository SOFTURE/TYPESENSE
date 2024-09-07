using System.Text.Json.Serialization;
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
}