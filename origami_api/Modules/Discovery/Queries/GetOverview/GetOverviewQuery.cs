using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Discovery.Contracts;

namespace Origami.Api.Modules.Discovery.Queries.GetOverview;

public sealed record GetOverviewQuery() : IRequest<DiscoveryOverviewDto>;

public sealed class GetOverviewHandler : IRequestHandler<GetOverviewQuery, DiscoveryOverviewDto>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetOverviewHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DiscoveryOverviewDto> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
    {
        return new DiscoveryOverviewDto(
            await _dbContext.Models.CountAsync(cancellationToken),
            await _dbContext.Creators.CountAsync(cancellationToken),
            await _dbContext.Images.CountAsync(cancellationToken),
            await _dbContext.Publications.CountAsync(cancellationToken),
            await _dbContext.CfcDiagrams.CountAsync(cancellationToken),
            await _dbContext.CfcBooks.CountAsync(cancellationToken),
            await _dbContext.CfcResources.CountAsync(cancellationToken),
            await _dbContext.CfcCalls.CountAsync(cancellationToken),
            await _dbContext.OrcModels.CountAsync(cancellationToken));
    }
}
