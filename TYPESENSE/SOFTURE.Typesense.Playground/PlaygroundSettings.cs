using SOFTURE.Typesense.Settings;

namespace SOFTURE.Typesense.Playground;

internal sealed class PlaygroundSettings : ITypesenseSettings
{
    public TypesenseSettings Typesense { get; init; }
}