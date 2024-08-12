using SOFTURE.Typesense.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleDocument() : DocumentBase(collection: "example")
{
    public override required string Id { get; set; }

    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public string? City { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Identifier: {Identifier}, City: {City}";
    }
}