using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Discovery.Contracts;

namespace Origami.Api.Modules.Discovery.Queries.GetFeatured;

public sealed record GetFeaturedQuery(int Limit = 6) : IRequest<DiscoveryFeaturedDto>;

public sealed class GetFeaturedHandler : IRequestHandler<GetFeaturedQuery, DiscoveryFeaturedDto>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetFeaturedHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DiscoveryFeaturedDto> Handle(GetFeaturedQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 20);

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

        return new DiscoveryFeaturedDto(topCreators, latestBooks, latestResources, latestCalls);
    }
}
