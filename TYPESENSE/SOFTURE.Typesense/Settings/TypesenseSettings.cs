namespace SOFTURE.Typesense.Settings;

public sealed class TypesenseSettings
{
#if NET8_0
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string ApiKey { get; init; }
#endif

#if NET6_0
    public string Host { get; init; } = null!;
    public string Port { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
#endif
}