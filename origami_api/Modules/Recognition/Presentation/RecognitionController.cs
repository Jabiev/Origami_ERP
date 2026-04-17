using MediatR;
using Microsoft.AspNetCore.Mvc;
using Origami.Api.Modules.Recognition.Commands.PredictImage;
using Origami.Api.Modules.Recognition.Contracts;

namespace Origami.Api.Modules.Recognition.Presentation;

[ApiController]
[Route("api/recognition")]
public sealed class RecognitionController : ControllerBase
{
    private readonly ISender _sender;

    public RecognitionController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("predict")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<RecognitionResponseDto>> Predict(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new RecognitionResponseDto(string.Empty, [], [], null, "An image file is required."));
        }

        var result = await _sender.Send(new PredictImageCommand(file), cancellationToken);
        return string.IsNullOrWhiteSpace(result.Error)
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}
