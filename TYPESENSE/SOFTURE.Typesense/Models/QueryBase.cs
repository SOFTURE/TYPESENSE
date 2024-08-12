using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Models;

public abstract class QueryBase(Collection collection) : SearchBase(collection)
{
    internal string Text()
    {
        var values = new List<string>();

        foreach (var property in GetProperties())
        {
            var value = property.GetValue(this);
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                values.Add(value.ToString()!);
            }
        }
        
        return values.Count == 0 
            ? "*" 
            : string.Join(", ", values);
    }

    internal string QueryBy()
    {
        var values = new List<string>();

        foreach (var property in GetProperties())
        {
            var value = property.GetValue(this);
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                values.Add(property.Name.ToLower());
            }
        }

        return values.Count == 0 
            ? "" 
            : string.Join(", ", values);
    }
}