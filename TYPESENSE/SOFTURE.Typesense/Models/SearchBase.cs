using System.Reflection;
using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Models;

public abstract class SearchBase(Collection collection)
{
    public Collection Collection => collection;
    
    internal bool CanSearch()
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
    
    protected List<PropertyInfo> GetProperties()
    {
        return GetType().GetProperties().Where(x => x.Name != "Collection").ToList();
    }
}