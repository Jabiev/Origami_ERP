namespace Origami.Api.Modules.Analytics.Contracts;

public sealed record NamedCountDto(string Name, int Count);

public sealed record DifficultyBandDto(string Level, string Label, int Count);

public sealed record ImageCoverageDto(int WithImage, int WithoutImage);

public sealed record SourceCoverageDto(string Source, int WithImage, int WithoutImage);

public sealed record ComplexityPointDto(string Difficulty, int Pieces, int Count);

public sealed record CreatorCountryDto(string Country, int Count);

public sealed record CreatorProductivityDto(string Bucket, int Count);

public sealed record AnalyticsLandscapeDto(
    IReadOnlyList<DifficultyBandDto> DifficultyDistribution,
    IReadOnlyList<NamedCountDto> TopPaperShapes,
    IReadOnlyList<NamedCountDto> TechniqueUsage,
    ImageCoverageDto ImageCoverage,
    IReadOnlyList<SourceCoverageDto> SourceCoverage,
    IReadOnlyList<ComplexityPointDto> ComplexityHotspots);

public sealed record CreatorAnalyticsDto(
    IReadOnlyList<NamedCountDto> TopCreators,
    IReadOnlyList<CreatorCountryDto> TopCountries,
    IReadOnlyList<CreatorProductivityDto> ProductivityDistribution);
