namespace Origami.Api.Modules.Catalog.Contracts;

public sealed record CatalogModelSummaryDto(
    Guid ModelId,
    string ModelNameOriginal,
    string? ModelNameNormalized,
    string? Difficulty,
    string? PaperShape,
    int? Pieces,
    bool? UsesCutting,
    bool? UsesGlue,
    string? SourceUrl,
    Guid? CreatorId,
    string? CreatorName,
    string? PrimaryImageUrl);

public sealed record CatalogImageDto(Guid ImageId, string? Url, string? CloudinaryUrl, bool IsPrimary, int? Width, int? Height, string? Angle);

public sealed record CatalogPublicationDto(Guid PublicationId, string Title, string? Type, string? Publisher, int? Year, string? Url, string? InstructionType, string? InstructionUrl, int? PageNumber);

public sealed record CatalogModelDetailsDto(
    Guid ModelId,
    string ModelNameOriginal,
    string? ModelNameNormalized,
    string? Difficulty,
    string? PaperShape,
    string? PaperToModelRatio,
    string? RecommendedPaperSize,
    int? Pieces,
    int? YearCreated,
    bool? IsAbstract,
    bool? UsesCutting,
    bool? UsesGlue,
    string? SourceUrl,
    Guid? CreatorId,
    string? CreatorName,
    IReadOnlyList<CatalogImageDto> Images,
    IReadOnlyList<CatalogPublicationDto> Publications);
