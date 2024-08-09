using System.Reflection;
using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Models;

public abstract class QueryBase(Collection collection)
{
    public Collection Collection => collection;

    public bool CanSearch()
    {
        foreach (var property in GetProperties())
        {
            var value = property.GetValue(this);
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }
        }
        
        return false;
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

        return string.Join(", ", values);
    }

    public string QueryBy()
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

        return string.Join(", ", values);
    }
    
    private List<PropertyInfo> GetProperties()
    {
        return GetType().GetProperties().Where(x => x.Name != "Collection").ToList();
    }
}