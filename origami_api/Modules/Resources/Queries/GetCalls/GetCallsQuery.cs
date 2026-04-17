using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Common;
using Origami.Api.Modules.Resources.Contracts;

namespace Origami.Api.Modules.Resources.Queries.GetCalls;

public sealed record GetCallsQuery(int Limit = 20) : IRequest<PagedResult<CallDto>>;

public sealed class GetCallsHandler : IRequestHandler<GetCallsQuery, PagedResult<CallDto>>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetCallsHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CallDto>> Handle(GetCallsQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);
        var items = await _dbContext.CfcCalls
            .AsNoTracking()
            .OrderByDescending(x => x.ScrapedAt)
            .Take(limit)
            .Select(x => new CallDto(x.Id, x.Title, x.Summary, x.Url, x.PostedOn, x.SubmissionDeadline))
            .ToListAsync(cancellationToken);

        return new PagedResult<CallDto>(items.Count, items);
    }
}
