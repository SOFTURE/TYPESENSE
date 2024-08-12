using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Models;
using SOFTURE.Typesense.ValueObjects;
using Typesense;

namespace SOFTURE.Typesense.Extensions;

public static class ConfigurationExtensions
{
    public static void ConfigureCollection<TDocument, TQuery, TFilters>(
        this List<CollectionConfiguration> configurations,
        string collectionName,
        List<Field> fields,
        string defaultSortingField)
        where TDocument : DocumentBase
        where TQuery : QueryBase
        where TFilters : FilterBase
    {
        var filterValidationResult = ValidateFilterModel<TFilters>(fields);
        var queryValidationResult = ValidateQueryModel<TQuery>(fields);

        Result.Combine(filterValidationResult, queryValidationResult)
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

    private static Result ValidateFilterModel<TFilters>(List<Field> fields)
        where TFilters : FilterBase
    {
        var facetFields = fields
            .Where(f => f.Facet.HasValue && f.Facet.Value)
            .Select(f => f.Name.ToLower())
            .ToList();

        var filterProperties = typeof(TFilters)
            .GetProperties()
            .Where(x => x.Name != "Collection")
            .Select(x => x.Name.ToLower())
            .ToList();

        var invalidFilters = filterProperties
            .Where(filterProperty => !facetFields.Contains(filterProperty))
            .ToList();

        return invalidFilters.Count != 0
            ? Result.Failure($"'{typeof(TFilters).Name}' missing '{string.Join(", ", invalidFilters)}' properties with 'Facet = true'")
            : Result.Success();
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