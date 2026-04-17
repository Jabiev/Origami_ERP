using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Common;
using Origami.Api.Modules.Resources.Contracts;
using Origami.Api.Modules.Resources.Queries.GetArticles;
using Origami.Api.Modules.Resources.Queries.GetBooks;
using Origami.Api.Modules.Resources.Queries.GetCalls;
using Origami.Api.Modules.Resources.Queries.GetDiagrams;

namespace Origami.Api.Modules.Resources.Presentation;

[ApiController]
[Route("api/resources")]
public sealed class ResourcesController : ControllerBase
{
    private readonly ISender _sender;

    public ResourcesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("books")]
    public async Task<ActionResult<PagedResult<BookDto>>> GetBooks([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
        => Ok(await _sender.Send(new GetBooksQuery(limit), cancellationToken));

    [HttpGet("diagrams")]
    public async Task<ActionResult<PagedResult<DiagramDto>>> GetDiagrams([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
        => Ok(await _sender.Send(new GetDiagramsQuery(limit), cancellationToken));

    [HttpGet("articles")]
    public async Task<ActionResult<PagedResult<ArticleDto>>> GetArticles([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
        => Ok(await _sender.Send(new GetArticlesQuery(limit), cancellationToken));

    [HttpGet("calls")]
    public async Task<ActionResult<PagedResult<CallDto>>> GetCalls([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
        => Ok(await _sender.Send(new GetCallsQuery(limit), cancellationToken));
}
