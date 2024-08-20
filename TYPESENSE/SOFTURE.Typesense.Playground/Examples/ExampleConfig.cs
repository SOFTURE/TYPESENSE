using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Extensions;
using SOFTURE.Typesense.ValueObjects;
using Typesense;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleConfig : ICollectionConfiguration
{
    public ExampleConfig()
    {
        // FACET - Fields with 'true' can be filtered & grouped - for example voivodeship, city, category, brand etc.

        Configurations.ConfigureCollection<ExampleDocument, ExampleQuery, ExampleFilters>(
            collectionName: "example",
            fields:
            [
                new Field("name", FieldType.String, facet: false, index: true, optional: false, sort: true),
                new Field("identifier", FieldType.String, facet: true),
                new Field("city", FieldType.String, facet: true)
            ],
            defaultSortingField: "name"
        );
    }

    public List<CollectionConfiguration> Configurations { get; } = [];
}