namespace Origami.Api.Modules.Discovery.Contracts;

public sealed record DiscoveryOverviewDto(
    int Models,
    int Creators,
    int Images,
    int Publications,
    int CfcDiagrams,
    int CfcBooks,
    int CfcResources,
    int CfcCalls,
    int OrcModels);

public sealed record FeaturedCreatorDto(Guid CreatorId, string NameOriginal, int ModelCount);

public sealed record FeaturedResourceDto(Guid Id, string Title, string? Subtitle, string? Url, string ResourceType);

public sealed record DiscoveryFeaturedDto(
    IReadOnlyList<FeaturedCreatorDto> TopCreators,
    IReadOnlyList<FeaturedResourceDto> LatestBooks,
    IReadOnlyList<FeaturedResourceDto> LatestResources,
    IReadOnlyList<FeaturedResourceDto> LatestCalls);
