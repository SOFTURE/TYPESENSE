using System.Reflection;
using System.Text.Json.Serialization;
using SOFTURE.Typesense.Abstractions.Models;
using Typesense;

namespace SOFTURE.Typesense.Utilities;

internal static class SchemaValidator
{
    public static void ValidateDocumentSchema<TDocument>(IReadOnlyList<Field> schemaFields)
        where TDocument : DocumentBase
    {
        var documentType = typeof(TDocument);

        var documentProperties = documentType
            .GetProperties()
            .Where(p => p.Name != nameof(DocumentBase.Collection))
            .Select(p => new
            {
                Property = p,
                FieldName = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? ToSnakeCase(p.Name)
            })
            .ToDictionary(x => x.FieldName, x => x.Property);

        var missingFields = new List<string>();

        foreach (var field in schemaFields.Where(f => !f.Optional.GetValueOrDefault(false)))
        {
            if (!documentProperties.ContainsKey(field.Name))
            {
                missingFields.Add(field.Name);
            }
        }

        if (missingFields.Count > 0)
        {
            throw new InvalidOperationException(
                $"Schema validation failed for '{documentType.Name}'. " +
                $"Required fields missing in document class: {string.Join(", ", missingFields.Select(f => $"'{f}'"))}. " +
                $"Please add these properties with [JsonPropertyName(\"{string.Join("\")], [JsonPropertyName(\"", missingFields)}\")] attributes.");
        }
    }

    private static string ToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(text[0]));

        for (var i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(text[i]));
            }
            else
            {
                result.Append(text[i]);
            }
        }

        return result.ToString();
    }
}
