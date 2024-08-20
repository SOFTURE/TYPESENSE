using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Models;

public abstract class FilterBase(Collection collection) : SearchBase(collection)
{
    internal string FilterBy()
    {
        var filterBy = GetProperties()
            .Select(property =>
            {
                var value = property.GetValue(this);
                var name = property.Name.ToLower();

                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    return $"{name}: {value}";
                }
                
                return string.Empty;
            });
        
        return string.Join(" && ", filterBy);
    }
}