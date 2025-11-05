namespace SOFTURE.Typesense.Abstractions.Models;

public abstract class SortBase : SearchBase
{
    private readonly Dictionary<string, SortDirection> _sortFields = new();

    protected SortBase(Collection collection) : base(collection)
    {
    }

    public void Add(string fieldName, SortDirection direction)
    {
        _sortFields[fieldName] = direction;
    }

    public string SortBy()
    {
        var sortBy = new List<string>();

        foreach (var (fieldName, direction) in _sortFields)
        {
            var directionStr = direction == SortDirection.Asc ? "asc" : "desc";
            sortBy.Add($"{fieldName}:{directionStr}");
        }

        return string.Join(",", sortBy);
    }
}
