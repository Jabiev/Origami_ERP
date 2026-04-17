using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Discovery.Contracts;
using Origami.Api.Modules.Recognition.Contracts;
using Origami.Api.Modules.Showcase.Contracts;

namespace Origami.Api.Modules.Showcase.Queries.GetShowcase;

public sealed record GetShowcaseQuery(int Limit = 6) : IRequest<ShowcaseResponseDto>;

public sealed class GetShowcaseHandler : IRequestHandler<GetShowcaseQuery, ShowcaseResponseDto>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetShowcaseHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShowcaseResponseDto> Handle(GetShowcaseQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 12);

        var overview = new DiscoveryOverviewDto(
            await _dbContext.Models.CountAsync(cancellationToken),
            await _dbContext.Creators.CountAsync(cancellationToken),
            await _dbContext.Images.CountAsync(cancellationToken),
            await _dbContext.Publications.CountAsync(cancellationToken),
            await _dbContext.CfcDiagrams.CountAsync(cancellationToken),
            await _dbContext.CfcBooks.CountAsync(cancellationToken),
            await _dbContext.CfcResources.CountAsync(cancellationToken),
            await _dbContext.CfcCalls.CountAsync(cancellationToken),
            await _dbContext.OrcModels.CountAsync(cancellationToken));

        var topCreators = await _dbContext.Creators
            .AsNoTracking()
            .Select(creator => new
            {
                creator.CreatorId,
                creator.NameOriginal,
                ModelCount = creator.Models.Count
            })
            .OrderByDescending(x => x.ModelCount)
            .ThenBy(x => x.NameOriginal)
            .Take(limit)
            .Select(x => new FeaturedCreatorDto(x.CreatorId, x.NameOriginal, x.ModelCount))
            .ToListAsync(cancellationToken);

        var latestBooks = await _dbContext.CfcBooks
            .OrderByDescending(x => x.ScrapedAt)
            .Select(x => new FeaturedResourceDto(x.Id, x.Title, x.Author, x.Url, "book"))
            .Take(limit)
            .ToListAsync(cancellationToken);

        var latestResources = await _dbContext.CfcResources
            .OrderByDescending(x => x.ScrapedAt)
            .Select(x => new FeaturedResourceDto(x.Id, x.Title, x.Summary, x.Url, "resource"))
            .Take(limit)
            .ToListAsync(cancellationToken);

        var latestCalls = await _dbContext.CfcCalls
            .OrderByDescending(x => x.ScrapedAt)
            .Select(x => new FeaturedResourceDto(x.Id, x.Title, x.SubmissionDeadline, x.Url, "call"))
            .Take(limit)
            .ToListAsync(cancellationToken);

        var featured = new DiscoveryFeaturedDto(topCreators, latestBooks, latestResources, latestCalls);

        var signatureModels = await _dbContext.Models
            .AsNoTracking()
            .Include(x => x.Creator)
            .Include(x => x.Images)
            .OrderByDescending(x => x.Images.Count)
            .ThenBy(x => x.ModelNameOriginal)
            .Take(limit)
            .Select(x => new ShowcaseModelDto(
                x.ModelId,
                x.ModelNameOriginal,
                x.Creator.NameOriginal,
                x.Difficulty,
                x.Images.Count,
                x.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.CloudinaryUrl ?? i.Url).FirstOrDefault(),
                x.SourceUrl))
            .ToListAsync(cancellationToken);

        var orcHighlights = await _dbContext.OrcModels
            .AsNoTracking()
            .Where(x => x.CloudinaryUrl != null || x.DiagramUrl != null)
            .OrderByDescending(x => x.ScrapedAt)
            .ThenBy(x => x.ModelName)
            .Take(limit)
            .Select(x => new ShowcaseOrcDto(
                x.Id,
                x.ModelName,
                x.CreatorExpanded,
                x.Category,
                x.DiagramType,
                x.DiagramIsHostedOnOrc,
                x.CloudinaryUrl,
                x.DiagramUrl))
            .ToListAsync(cancellationToken);

        var recognitionExample = signatureModels
            .Take(3)
            .Select((model, index) => new RecognitionPredictionDto(index, model.ModelNameOriginal, Math.Round(97.5 - (index * 6.25), 2)))
            .ToList();

        return new ShowcaseResponseDto(overview, featured, signatureModels, orcHighlights, recognitionExample);
    }
}
