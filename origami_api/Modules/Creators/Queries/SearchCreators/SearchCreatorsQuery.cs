using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Common;
using Origami.Api.Modules.Creators.Contracts;

namespace Origami.Api.Modules.Creators.Queries.SearchCreators;

public sealed record SearchCreatorsQuery(string? Query, int Limit = 20) : IRequest<PagedResult<CreatorSummaryDto>>;

public sealed class SearchCreatorsHandler : IRequestHandler<SearchCreatorsQuery, PagedResult<CreatorSummaryDto>>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public SearchCreatorsHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CreatorSummaryDto>> Handle(SearchCreatorsQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);
        var query = _dbContext.Creators.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var search = request.Query.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.NameOriginal, $"%{search}%") ||
                (x.NameNormalized != null && EF.Functions.ILike(x.NameNormalized, $"%{search}%")) ||
                x.CreatorAliases.Any(a => EF.Functions.ILike(a.Alias, $"%{search}%")));
        }

        var items = await query
            .OrderBy(x => x.NameOriginal)
            .Take(limit)
            .Select(x => new CreatorSummaryDto(
                x.CreatorId,
                x.NameOriginal,
                x.NameNormalized,
                x.Country,
                x.Language,
                x.Models.Count))
            .ToListAsync(cancellationToken);

        return new PagedResult<CreatorSummaryDto>(items.Count, items);
    }
}
