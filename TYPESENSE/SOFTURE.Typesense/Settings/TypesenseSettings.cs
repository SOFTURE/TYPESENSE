namespace SOFTURE.Typesense.Settings;

public sealed class TypesenseSettings
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string ApiKey { get; init; }
}