namespace SOFTURE.Typesense.Abstractions.Models;

public abstract class DocumentBase
{
    protected DocumentBase(Collection collection)
    {
        Collection = collection;
    }
   
    public Collection Collection { get; }

#if NET8_0
    public abstract required string Id { get; set; }
#endif

#if NET6_0
    public abstract string Id { get; set; }
#endif
}