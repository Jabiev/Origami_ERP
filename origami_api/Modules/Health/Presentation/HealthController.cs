using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Health.Contracts;
using Origami.Api.Modules.Health.Queries.GetHealth;

namespace Origami.Api.Modules.Health.Presentation;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    private readonly ISender _sender;

    public HealthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<HealthDto>> Get(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetHealthQuery(), cancellationToken));
    }
}
