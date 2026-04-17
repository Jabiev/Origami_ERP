using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Analytics.Contracts;
using Origami.Api.Modules.Analytics.Queries.GetCreatorAnalytics;
using Origami.Api.Modules.Analytics.Queries.GetLandscape;

namespace Origami.Api.Modules.Analytics.Presentation;

[ApiController]
[Route("api/analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly ISender _sender;

    public AnalyticsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("landscape")]
    public async Task<ActionResult<AnalyticsLandscapeDto>> GetLandscape(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetLandscapeQuery(), cancellationToken));
    }

    [HttpGet("creators")]
    public async Task<ActionResult<CreatorAnalyticsDto>> GetCreators(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetCreatorAnalyticsQuery(), cancellationToken));
    }
}
