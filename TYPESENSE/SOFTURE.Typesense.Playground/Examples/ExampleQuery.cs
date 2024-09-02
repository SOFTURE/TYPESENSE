using SOFTURE.Typesense.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleQuery() : QueryBase(collection: "example")
{
    public string? Name { get; set; }
}