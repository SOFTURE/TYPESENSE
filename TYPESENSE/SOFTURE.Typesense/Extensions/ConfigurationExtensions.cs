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
        var result = Collection.Create(collectionName)
            .Bind(collection => CollectionConfiguration.Create<TDocument>(
                collection: collection,
                fields: fields,
                defaultSortingField: defaultSortingField
            ));

        if (result.IsFailure)
            throw new InvalidOperationException($"[TYPESENSE] Failed to configure collection '{collectionName}': {result.Error}");

        configurations.Add(result.Value);
    }
}