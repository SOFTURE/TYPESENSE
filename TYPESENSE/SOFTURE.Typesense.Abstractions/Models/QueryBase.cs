using System.Reflection;
using System.Text.Json.Serialization;

namespace SOFTURE.Typesense.Abstractions.Models;

public abstract class QueryBase : SearchBase
{
    protected QueryBase(Collection collection) : base(collection)
    {
    }
    
    public string Text()
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

    public string QueryBy()
    {
        var values = new List<string>();

        foreach (var property in GetProperties())
        {
            var value = property.GetValue(this);
            if (value == null || string.IsNullOrEmpty(value.ToString())) 
                continue;
            
            var name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name.ToLower();
            values.Add(name);
        }

        return values.Count == 0
            ? ""
            : string.Join(", ", values);
    }
}