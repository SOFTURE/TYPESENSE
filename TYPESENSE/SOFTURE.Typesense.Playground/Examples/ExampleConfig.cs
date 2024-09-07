using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Extensions;
using SOFTURE.Typesense.ValueObjects;
using Typesense;

namespace SOFTURE.Typesense.Playground.Examples;

public sealed class ExampleConfig : ICollectionConfiguration
{
    public ExampleConfig()
    {
        // FACET (default: false) - Fields with 'true' can be filtered & grouped - for example voivodeship, city, category, brand etc.
        // INDEX (default: true) - Set to 'false' for fields that should not be searchable - for example 'description', 'content' etc.
        // INFIX (default: false) - Memory consuming, but allows to search for parts of the word - for example 'car' will find 'car', 'cars', 'caravan' etc.
        
        Configurations.ConfigureCollection<ExampleDocument, ExampleQuery, ExampleFilters>(
            collectionName: "example",
            fields:
            [
                new Field("name", FieldType.String, facet: false, index: true, optional: false, sort: true),
                new Field("identifier", FieldType.String, facet: false),
                new Field("city", FieldType.String, facet: true),
                new Field("is_active", FieldType.Bool, facet: true),
                new Field("voivodeship_id", FieldType.Int32, facet: true)
            ],
            defaultSortingField: "name"
        );
    }

    public List<CollectionConfiguration> Configurations { get; } = [];
}