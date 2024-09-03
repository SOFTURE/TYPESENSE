using SOFTURE.Typesense.Abstractions.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleFilters() : FilterBase(collection: "example")
{
    public string? City { get; set; }
    public string? Identifier { get; set; }
}