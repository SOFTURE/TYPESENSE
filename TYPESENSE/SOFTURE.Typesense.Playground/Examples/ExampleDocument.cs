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
    
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Identifier: {Identifier}, City: {City}, IsActive: {IsActive}, VoivodeshipId: {VoivodeshipId}";
    }
}