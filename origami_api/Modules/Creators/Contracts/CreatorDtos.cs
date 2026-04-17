namespace Origami.Api.Modules.Creators.Contracts;

public sealed record CreatorSummaryDto(Guid CreatorId, string NameOriginal, string? NameNormalized, string? Country, string? Language, int ModelCount);

public sealed record CreatorAliasDto(Guid AliasId, string Alias, string? Language, string? Source);

public sealed record CreatorModelDto(Guid ModelId, string ModelNameOriginal, string? Difficulty, string? PrimaryImageUrl);

public sealed record CreatorDetailsDto(
    Guid CreatorId,
    string NameOriginal,
    string? NameNormalized,
    string? Biography,
    string? Country,
    string? Language,
    int? BirthYear,
    int? DeathYear,
    IReadOnlyList<CreatorAliasDto> Aliases,
    IReadOnlyList<CreatorModelDto> Models);
