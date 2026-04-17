namespace Origami.Api.Modules.Recognition.Contracts;

public sealed record RecognitionPredictionDto(int ClassIndex, string Label, double Confidence);

public sealed record RecognitionMatchedModelDto(
    Guid ModelId,
    string ModelNameOriginal,
    Guid? CreatorId,
    string? CreatorName,
    string? Difficulty,
    string? PaperShape,
    string? SourceUrl,
    string? PrimaryImageUrl);

public sealed record RecognitionResultDto(
    string FileName,
    IReadOnlyList<RecognitionPredictionDto> Predictions,
    string? RawOutput,
    string? Error);

public sealed record RecognitionResponseDto(
    string FileName,
    IReadOnlyList<RecognitionPredictionDto> Predictions,
    IReadOnlyList<RecognitionMatchedModelDto> MatchedModels,
    string? RawOutput,
    string? Error);
