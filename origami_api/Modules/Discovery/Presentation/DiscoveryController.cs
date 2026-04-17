using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Discovery.Contracts;
using Origami.Api.Modules.Discovery.Queries.GetFeatured;
using Origami.Api.Modules.Discovery.Queries.GetOverview;

namespace Origami.Api.Modules.Discovery.Presentation;

[ApiController]
[Route("api/discovery")]
public sealed class DiscoveryController : ControllerBase
{
    private readonly ISender _sender;

    public DiscoveryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<DiscoveryOverviewDto>> GetOverview(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetOverviewQuery(), cancellationToken));
    }

    [HttpGet("featured")]
    public async Task<ActionResult<DiscoveryFeaturedDto>> GetFeatured([FromQuery] int limit = 6, CancellationToken cancellationToken = default)
    {
        return Ok(await _sender.Send(new GetFeaturedQuery(limit), cancellationToken));
    }
}
