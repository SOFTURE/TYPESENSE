using SOFTURE.Typesense.Abstractions.Models;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleSort() : SortBase(collection: "example")
{
    public ExampleSort WithName(SortDirection direction)
    {
        Add("name", direction);
        return this;
    }

    public ExampleSort WithCity(SortDirection direction)
    {
        Add("city", direction);
        return this;
    }

    public ExampleSort WithIdentifier(SortDirection direction)
    {
        Add("identifier", direction);
        return this;
    }

    public ExampleSort WithVoivodeshipId(SortDirection direction)
    {
        Add("voivodeship_id", direction);
        return this;
    }
}