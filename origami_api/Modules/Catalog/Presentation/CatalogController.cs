using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Catalog.Contracts;
using Origami.Api.Modules.Catalog.Queries.GetModelDetails;
using Origami.Api.Modules.Catalog.Queries.SearchModels;
using Origami.Api.Modules.Common;

namespace Origami.Api.Modules.Catalog.Presentation;

[ApiController]
[Route("api/catalog/models")]
public sealed class CatalogController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CatalogModelSummaryDto>>> Search(
        [FromQuery] string? query,
        [FromQuery] Guid? creatorId,
        [FromQuery] string? difficulty,
        [FromQuery] string? paperShape,
        [FromQuery] bool? usesCutting,
        [FromQuery] bool? usesGlue,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _sender.Send(new SearchModelsQuery(query, creatorId, difficulty, paperShape, usesCutting, usesGlue, limit), cancellationToken));
    }

    [HttpGet("{modelId:guid}")]
    public async Task<ActionResult<CatalogModelDetailsDto>> GetById(Guid modelId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetModelDetailsQuery(modelId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
