using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Interfaces;

public interface ICollectionClient
{
    Task<Result<CreationState>> CreateCollection(CollectionConfiguration configuration);
    Task<Result> DeleteCollection(Collection collection);
}