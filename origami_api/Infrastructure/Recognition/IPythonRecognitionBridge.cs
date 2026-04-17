using Origami.Api.Modules.Recognition.Contracts;

namespace Origami.Api.Infrastructure.Recognition;

public interface IPythonRecognitionBridge
{
    Task<RecognitionResultDto> PredictAsync(string fileName, Stream imageStream, CancellationToken cancellationToken);
}
