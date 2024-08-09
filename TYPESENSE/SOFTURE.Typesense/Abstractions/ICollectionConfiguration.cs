using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Abstractions;

public interface ICollectionConfiguration
{
    List<CollectionConfiguration> Configurations { get; }
}