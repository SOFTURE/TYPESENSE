using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Models;

public abstract class DocumentBase(Collection collection)
{
    public Collection Collection => collection;

#if NET8_0
    public abstract required string Id { get; set; }
#endif

#if NET6_0
    public abstract string Id { get; set; }
#endif
}