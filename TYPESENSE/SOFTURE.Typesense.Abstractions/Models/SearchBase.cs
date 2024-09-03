using System.Reflection;

namespace SOFTURE.Typesense.Abstractions.Models;

public abstract class SearchBase
{
    protected SearchBase(Collection collection)
    {
        Collection = collection;
    }
    
    public Collection Collection { get; }
    
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