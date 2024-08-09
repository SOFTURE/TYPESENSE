using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.ValueObjects;
using Typesense;

namespace SOFTURE.Typesense.Playground.Examples;

public class ExampleConfig : ICollectionConfiguration
{
    public ExampleConfig()
    {
        Collection.Create("example")
            .Bind(collection => CollectionConfiguration.Create<ExampleDocument>(
                collection: collection,
                fields: new List<Field>
                {
                    new("name", FieldType.String, facet: false, index: true, optional: false, sort: true),
                    new("city", FieldType.String)
                },
                defaultSortingField: "name"
            ))
            .Tap(configuration => Configurations.Add(configuration))
            .TapError(error => Console.WriteLine($"[TYPESENSE][ERROR] {error}"));
    }
    
    public List<CollectionConfiguration> Configurations { get; } = [];
}