using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Common;
using Origami.Api.Modules.Resources.Contracts;

namespace Origami.Api.Modules.Resources.Queries.GetArticles;

public sealed record GetArticlesQuery(int Limit = 20) : IRequest<PagedResult<ArticleDto>>;

public sealed class GetArticlesHandler : IRequestHandler<GetArticlesQuery, PagedResult<ArticleDto>>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetArticlesHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ArticleDto>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);
        var items = await _dbContext.CfcResources
            .AsNoTracking()
            .OrderByDescending(x => x.ScrapedAt)
            .Take(limit)
            .Select(x => new ArticleDto(x.Id, x.Title, x.Summary, x.Url, x.PostedOn, x.UpdatedDate))
            .ToListAsync(cancellationToken);

        return new PagedResult<ArticleDto>(items.Count, items);
    }
}
