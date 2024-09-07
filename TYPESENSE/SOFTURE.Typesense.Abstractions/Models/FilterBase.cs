using System.Reflection;
using System.Text.Json.Serialization;

namespace SOFTURE.Typesense.Abstractions.Models;

public abstract class FilterBase : SearchBase
{
    protected FilterBase(Collection collection) : base(collection)
    {
    }
    
    public string FilterBy()
    {
        var filterBy = new List<string>();
        
        foreach (var property in GetProperties())
        {
            var value = property.GetValue(this);
            var name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                filterBy.Add($"{name}: {value.ToString()!.ToLower()}");
            }
        }
        
        return string.Join(" && ", filterBy);
    }
}