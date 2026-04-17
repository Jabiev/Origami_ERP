using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Analytics.Contracts;

namespace Origami.Api.Modules.Analytics.Queries.GetLandscape;

public sealed record GetLandscapeQuery() : IRequest<AnalyticsLandscapeDto>;

public sealed class GetLandscapeHandler : IRequestHandler<GetLandscapeQuery, AnalyticsLandscapeDto>
{
    private static readonly IReadOnlyDictionary<string, string> DifficultyLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["1"] = "Simple",
        ["2"] = "Medium",
        ["3"] = "Intermediate",
        ["4"] = "Complex",
        ["5"] = "Super Complex"
    };

    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetLandscapeHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnalyticsLandscapeDto> Handle(GetLandscapeQuery request, CancellationToken cancellationToken)
    {
        var difficultyRaw = await _dbContext.Models
            .AsNoTracking()
            .Where(x => x.Difficulty != null && x.Difficulty != "")
            .GroupBy(x => x.Difficulty!)
            .Select(group => new { Level = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        var difficultyDistribution = difficultyRaw
            .OrderBy(x => x.Level)
            .Select(x => new DifficultyBandDto(x.Level, DifficultyLabels.GetValueOrDefault(x.Level, x.Level.ToString()), x.Count))
            .ToList();

        var topPaperShapes = await _dbContext.Models
            .AsNoTracking()
            .Where(x => x.PaperShape != null && x.PaperShape != "")
            .GroupBy(x => x.PaperShape!)
            .Select(group => new
            {
                Name = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Name)
            .Take(10)
            .Select(x => new NamedCountDto(x.Name, x.Count))
            .ToListAsync(cancellationToken);

        var models = await _dbContext.Models
            .AsNoTracking()
            .Select(x => new { x.ModelId, x.UsesCutting, x.UsesGlue, x.SourceUrl })
            .ToListAsync(cancellationToken);

        var imagesByModel = await _dbContext.Images
            .AsNoTracking()
            .GroupBy(x => x.ModelId)
            .Select(group => new { ModelId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(x => x.ModelId, x => x.Count, cancellationToken);

        var cutsOnly = models.Count(x => x.UsesCutting && !x.UsesGlue);
        var glueOnly = models.Count(x => !x.UsesCutting && x.UsesGlue);
        var both = models.Count(x => x.UsesCutting && x.UsesGlue);
        var neither = models.Count(x => !x.UsesCutting && !x.UsesGlue);

        var techniqueUsage = new List<NamedCountDto>
        {
            new("Cuts Only", cutsOnly),
            new("Glue Only", glueOnly),
            new("Cuts + Glue", both),
            new("Neither", neither)
        };

        var withImage = models.Count(x => imagesByModel.ContainsKey(x.ModelId));
        var withoutImage = models.Count - withImage;

        var sourceCoverage = models
            .GroupBy(x => ResolveSourceBucket(x.SourceUrl))
            .Select(group => new SourceCoverageDto(
                group.Key,
                group.Count(x => imagesByModel.ContainsKey(x.ModelId)),
                group.Count(x => !imagesByModel.ContainsKey(x.ModelId))))
            .OrderBy(x => x.Source)
            .ToList();

        var complexityHotspots = await _dbContext.Models
            .AsNoTracking()
            .Where(x => x.Difficulty != null && x.Difficulty != "" && x.Pieces != null && x.Pieces <= 20)
            .GroupBy(x => new { Difficulty = x.Difficulty!, Pieces = x.Pieces!.Value })
            .Select(group => new
            {
                group.Key.Difficulty,
                group.Key.Pieces,
                Count = group.Count()
            })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Difficulty)
            .ThenBy(x => x.Pieces)
            .Take(20)
            .Select(x => new ComplexityPointDto(x.Difficulty, x.Pieces, x.Count))
            .ToListAsync(cancellationToken);

        return new AnalyticsLandscapeDto(
            difficultyDistribution,
            topPaperShapes,
            techniqueUsage,
            new ImageCoverageDto(withImage, withoutImage),
            sourceCoverage,
            complexityHotspots);
    }

    private static string ResolveSourceBucket(string? sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
        {
            return "Unknown";
        }

        var normalized = sourceUrl.ToLowerInvariant();
        if (normalized.Contains("origami-resource-center"))
        {
            return "ORC";
        }

        if (normalized.Contains("cfc"))
        {
            return "CFC";
        }

        if (normalized.Contains("oriwiki"))
        {
            return "ORIWiki";
        }

        return "Other";
    }
}
