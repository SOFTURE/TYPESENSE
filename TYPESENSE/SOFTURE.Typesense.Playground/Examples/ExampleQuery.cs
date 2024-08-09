using SOFTURE.Typesense.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public class ExampleQuery() : QueryBase(collection: "example")
{
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public string? City { get; set; }
}