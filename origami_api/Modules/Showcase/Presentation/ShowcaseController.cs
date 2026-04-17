using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Showcase.Contracts;
using Origami.Api.Modules.Showcase.Queries.GetShowcase;

namespace Origami.Api.Modules.Showcase.Presentation;

[ApiController]
[Route("api/showcase")]
public sealed class ShowcaseController : ControllerBase
{
    private readonly ISender _sender;

    public ShowcaseController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("hero")]
    public async Task<ActionResult<ShowcaseResponseDto>> GetHero([FromQuery] int limit = 6, CancellationToken cancellationToken = default)
    {
        return Ok(await _sender.Send(new GetShowcaseQuery(limit), cancellationToken));
    }
}
