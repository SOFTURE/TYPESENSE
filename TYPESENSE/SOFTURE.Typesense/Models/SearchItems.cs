namespace SOFTURE.Typesense.Models;

public sealed class SearchItems<TDocument>
    where TDocument : DocumentBase
{
    private SearchItems(List<TDocument> items, int found, int total, int page)
    {
        Items = items;
        Found = found;
        Total = total;
        Page = page;
    }
    
    public List<TDocument> Items { get; }
    
    public int Found { get; }
    public int Total { get; }
    public int Page { get; }

    public override string ToString()
    {
        return $"Found: {Found}, Total: {Total}, Page: {Page}";
    }
    
    public static SearchItems<TDocument> Create(List<TDocument> items, int found, int total, int page)
    {
        return new SearchItems<TDocument>(items, found, total, page);
    }
}