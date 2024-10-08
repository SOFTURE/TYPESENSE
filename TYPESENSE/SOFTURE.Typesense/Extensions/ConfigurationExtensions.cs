using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Abstractions.Models;
using SOFTURE.Typesense.ValueObjects;
using Typesense;

namespace SOFTURE.Typesense.Extensions;

public static class ConfigurationExtensions
{
    public static void ConfigureCollection<TDocument, TQuery, TFilters>(
        this List<CollectionConfiguration> configurations,
        string collectionName,
        List<Field> fields,
        string? defaultSortingField = null)
        where TDocument : DocumentBase
        where TQuery : QueryBase
        where TFilters : FilterBase
    {
        Collection.Create(collectionName)
            .Bind(collection => CollectionConfiguration.Create<TDocument>(
                collection: collection,
                fields: fields,
                defaultSortingField: defaultSortingField
            ))
            .Tap(configurations.Add)
            .TapError(error => Console.WriteLine($"[TYPESENSE][ERROR] {error}"));
    }
}