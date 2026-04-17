using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Common;
using Origami.Api.Modules.Creators.Contracts;
using Origami.Api.Modules.Creators.Queries.GetCreatorDetails;
using Origami.Api.Modules.Creators.Queries.SearchCreators;

namespace Origami.Api.Modules.Creators.Presentation;

[ApiController]
[Route("api/creators")]
public sealed class CreatorsController : ControllerBase
{
    private readonly ISender _sender;

    public CreatorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CreatorSummaryDto>>> Search([FromQuery] string? query, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        return Ok(await _sender.Send(new SearchCreatorsQuery(query, limit), cancellationToken));
    }

    [HttpGet("{creatorId:guid}")]
    public async Task<ActionResult<CreatorDetailsDto>> GetById(Guid creatorId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCreatorDetailsQuery(creatorId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
