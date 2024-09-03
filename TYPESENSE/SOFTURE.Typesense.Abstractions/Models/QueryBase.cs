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