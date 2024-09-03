using CSharpFunctionalExtensions;
using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Abstractions;

public interface ICollectionClient
{
    Task<Result<CreationState>> CreateCollection(CollectionConfiguration configuration);
    Task<Result> DeleteCollection(Collection collection);
}