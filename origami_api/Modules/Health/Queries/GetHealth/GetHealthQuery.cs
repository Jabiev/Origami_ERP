using MediatR;
using Origami.Api.Modules.Health.Contracts;

namespace Origami.Api.Modules.Health.Queries.GetHealth;

public sealed record GetHealthQuery() : IRequest<HealthDto>;

public sealed class GetHealthHandler : IRequestHandler<GetHealthQuery, HealthDto>
{
    public Task<HealthDto> Handle(GetHealthQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HealthDto("ok", "origami_api", "1.0.0", DateTimeOffset.UtcNow));
    }
}
