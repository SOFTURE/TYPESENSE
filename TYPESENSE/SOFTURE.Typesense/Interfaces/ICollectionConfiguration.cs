using SOFTURE.Typesense.ValueObjects;

namespace SOFTURE.Typesense.Interfaces;

public interface ICollectionConfiguration
{
    List<CollectionConfiguration> Configurations { get; }
}