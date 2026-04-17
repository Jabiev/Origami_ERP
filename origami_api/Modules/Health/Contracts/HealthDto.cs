namespace Origami.Api.Modules.Health.Contracts;

public sealed record HealthDto(string Status, string Service, string Version, DateTimeOffset TimestampUtc);
