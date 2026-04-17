using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Common;
using Origami.Api.Modules.Resources.Contracts;

namespace Origami.Api.Modules.Resources.Queries.GetDiagrams;

public sealed record GetDiagramsQuery(int Limit = 20) : IRequest<PagedResult<DiagramDto>>;

public sealed class GetDiagramsHandler : IRequestHandler<GetDiagramsQuery, PagedResult<DiagramDto>>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetDiagramsHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DiagramDto>> Handle(GetDiagramsQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);
        var items = await _dbContext.CfcDiagrams
            .AsNoTracking()
            .OrderByDescending(x => x.ScrapedAt)
            .Take(limit)
            .Select(x => new DiagramDto(x.Id, x.Title, x.Creator, x.Category, x.Difficulty, x.Url, x.CloudinaryUrl))
            .ToListAsync(cancellationToken);

        return new PagedResult<DiagramDto>(items.Count, items);
    }
}
