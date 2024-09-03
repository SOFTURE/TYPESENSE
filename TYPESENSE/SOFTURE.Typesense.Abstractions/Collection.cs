using CSharpFunctionalExtensions;

namespace SOFTURE.Typesense.Abstractions;

public sealed class Collection : ValueObject
{
    private Collection(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static Result<Collection> Create(string name)
    {
        return string.IsNullOrWhiteSpace(name) 
            ? Result.Failure<Collection>("Collection name cannot be empty") 
            : new Collection(name);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Name;
    }
    
    public static implicit operator string(Collection collection) => collection.Name;
    
    public static implicit operator Collection(string name) => Create(name).Value;
}