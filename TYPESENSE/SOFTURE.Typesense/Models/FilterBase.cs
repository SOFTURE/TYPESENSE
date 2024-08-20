using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Models;

public abstract class FilterBase(Collection collection) : SearchBase(collection)
{
    internal string FilterBy()
    {
        
        var filterBy = new List<string>();
        
        foreach (var property in GetProperties())
        {
            var value = property.GetValue(this);
            var name = property.Name.ToLower();

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                filterBy.Add($"{name}: {value}");
            }
        }
        
        return string.Join(" && ", filterBy);
    }
}