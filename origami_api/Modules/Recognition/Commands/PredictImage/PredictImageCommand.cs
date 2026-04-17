using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Infrastructure.Recognition;
using Origami.Api.Modules.Recognition.Contracts;

namespace Origami.Api.Modules.Recognition.Commands.PredictImage;

public sealed record PredictImageCommand(IFormFile File) : IRequest<RecognitionResponseDto>;

public sealed class PredictImageHandler : IRequestHandler<PredictImageCommand, RecognitionResponseDto>
{
    private readonly IPythonRecognitionBridge _recognitionBridge;
    private readonly OrigamiScaffoldDbContext _dbContext;

    public PredictImageHandler(IPythonRecognitionBridge recognitionBridge, OrigamiScaffoldDbContext dbContext)
    {
        _recognitionBridge = recognitionBridge;
        _dbContext = dbContext;
    }

    public async Task<RecognitionResponseDto> Handle(PredictImageCommand request, CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();
        var result = await _recognitionBridge.PredictAsync(request.File.FileName, stream, cancellationToken);

        var labels = result.Predictions
            .Select(x => x.Label)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matchedModels = labels.Count == 0
            ? new List<RecognitionMatchedModelDto>()
            : await _dbContext.Models
                .AsNoTracking()
                .Include(x => x.Creator)
                .Include(x => x.Images)
                .Where(x => labels.Contains(x.ModelNameOriginal) || (x.ModelNameNormalized != null && labels.Contains(x.ModelNameNormalized)))
                .OrderBy(x => x.ModelNameOriginal)
                .Select(x => new RecognitionMatchedModelDto(
                    x.ModelId,
                    x.ModelNameOriginal,
                    x.CreatorId,
                    x.Creator != null ? x.Creator.NameOriginal : null,
                    x.Difficulty,
                    x.PaperShape,
                    x.SourceUrl,
                    x.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.CloudinaryUrl ?? i.Url).FirstOrDefault()))
                .ToListAsync(cancellationToken);

        return new RecognitionResponseDto(
            result.FileName,
            result.Predictions,
            matchedModels,
            result.RawOutput,
            result.Error);
    }
}
