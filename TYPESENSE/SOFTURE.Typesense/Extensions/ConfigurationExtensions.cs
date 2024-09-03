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
        var queryValidationResult = ValidateQueryModel<TQuery>(fields);

        Result.Combine(queryValidationResult)
            .Bind(() =>
            {
                return Collection.Create(collectionName)
                    .Bind(collection => CollectionConfiguration.Create<TDocument>(
                        collection: collection,
                        fields: fields,
                        defaultSortingField: defaultSortingField
                    ));
            })
            .Tap(configurations.Add)
            .TapError(error => Console.WriteLine($"[TYPESENSE][ERROR] {error}"));
    }
    
    private static Result ValidateQueryModel<TQuery>(List<Field> fields)
        where TQuery : QueryBase
    {
        var searchFields = fields
            .Select(f => f.Name.ToLower())
            .ToList();

        var queryProperties = typeof(TQuery)
            .GetProperties()
            .Where(x => x.Name != "Collection")
            .Select(x => x.Name.ToLower())
            .ToList();

        var invalidQueries = queryProperties
            .Where(queryProperty => !searchFields.Contains(queryProperty))
            .ToList();

        return invalidQueries.Count != 0
            ? Result.Failure($"'{typeof(TQuery).Name}' missing '{string.Join(", ", invalidQueries)}' properties in fields")
            : Result.Success();
    }
}