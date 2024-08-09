using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Models;
using Typesense;

namespace SOFTURE.Typesense.ValueObjects;

public sealed class CollectionConfiguration : ValueObject
{
    private CollectionConfiguration(
        Collection collection,
        IReadOnlyList<Field> fields,
        Field? defaultSortingField = null)
    {
        Collection = collection;
        Fields = fields;
        DefaultSortingField = defaultSortingField;
    }

    public Collection Collection { get; }
    private IReadOnlyList<Field> Fields { get; }
    private Field? DefaultSortingField { get; }

    public static Result<CollectionConfiguration> Create<TDocument>(
        Collection collection,
        IReadOnlyList<Field> fields,
        string? defaultSortingField)
        where TDocument : DocumentBase
    {
        var validationResult = ValidateDocumentStructure<TDocument>(fields);
        if (validationResult.IsFailure)
            return Result.Failure<CollectionConfiguration>(validationResult.Error);

        if (string.IsNullOrEmpty(defaultSortingField))
            return new CollectionConfiguration(collection, fields);

        var defaultField = fields.SingleOrDefault(f => f.Name == defaultSortingField);
        return defaultField == null
            ? Result.Failure<CollectionConfiguration>($"Sorting field '{defaultSortingField}' not found in fields.")
            : new CollectionConfiguration(collection, fields, defaultField);
    }

    internal Schema GetSchema()
    {
        if (DefaultSortingField == null)
        {
            return new Schema(name: Collection, fields: Fields);
        }

        return new Schema(
            name: Collection,
            fields: Fields,
            defaultSortingField: DefaultSortingField.Name);
    }

    internal bool ISchemaModified(IReadOnlyCollection<Field>? existingFields)
    {
        if (existingFields == null || existingFields.Count != Fields.Count) return true;

        var changedFields = Fields.Where(current => existingFields.All(
                    existing => existing.Name != current.Name
                                || existing.Type != current.Type
                                || existing.Facet.GetValueOrDefault(false) != current.Facet.GetValueOrDefault(false)
                                || existing.Index.GetValueOrDefault(true) != current.Index.GetValueOrDefault(true)
                                || existing.Optional.GetValueOrDefault(false) != current.Optional.GetValueOrDefault(false)
                                || existing.Sort.GetValueOrDefault(false) != current.Sort.GetValueOrDefault(false)
                                || existing.Infix.GetValueOrDefault(false) != current.Infix.GetValueOrDefault(false)
                                || (existing.Locale ?? "") != (current.Locale ?? "")
                )
            )
            .ToList();

        return changedFields.Count != 0;
    }

    private static Result ValidateDocumentStructure<TDocument>(IReadOnlyList<Field> fields)
        where TDocument : DocumentBase
    {
        var documentProperties = typeof(TDocument)
            .GetProperties()
            .Where(p => p.Name != "Collection" && p.Name != "Id")
            .Select(x => x.Name.ToLower());

        var fieldNames = fields.Select(f => f.Name.ToLower());

        var missingProperties = fieldNames.Except(documentProperties).ToList();

        return missingProperties.Count != 0
            ? Result.Failure($"Missing properties: '{string.Join(",", missingProperties)}' in document model: '{typeof(TDocument).Name}'.")
            : Result.Success();
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Collection;
    }
}