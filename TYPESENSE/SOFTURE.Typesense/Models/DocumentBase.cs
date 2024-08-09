
using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Models;

public abstract class DocumentBase(Collection collection)
{
    public Collection Collection => collection;
    public abstract required string Id { get; set; }
}