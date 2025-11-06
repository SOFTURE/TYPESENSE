using System.Text.Json.Serialization;
using SOFTURE.Typesense.Abstractions.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleQuery() : QueryBase(collection: "example")
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}