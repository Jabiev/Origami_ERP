using Origami.Api.Modules.Discovery.Contracts;
using Origami.Api.Modules.Recognition.Contracts;

namespace Origami.Api.Modules.Showcase.Contracts;

public sealed record ShowcaseModelDto(
    Guid ModelId,
    string ModelNameOriginal,
    string? CreatorName,
    string? Difficulty,
    int ImageCount,
    string? PrimaryImageUrl,
    string? SourceUrl);

public sealed record ShowcaseOrcDto(
    long Id,
    string? ModelName,
    string? CreatorExpanded,
    string? Category,
    string? DiagramType,
    bool? DiagramIsHostedOnOrc,
    string? CloudinaryUrl,
    string? DiagramUrl);

public sealed record ShowcaseResponseDto(
    DiscoveryOverviewDto Overview,
    DiscoveryFeaturedDto Featured,
    IReadOnlyList<ShowcaseModelDto> SignatureModels,
    IReadOnlyList<ShowcaseOrcDto> OrcHighlights,
    IReadOnlyList<RecognitionPredictionDto> RecognitionExample);
