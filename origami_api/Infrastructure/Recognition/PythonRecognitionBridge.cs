using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Origami.Api.Configuration;
using Origami.Api.Modules.Recognition.Contracts;

namespace Origami.Api.Infrastructure.Recognition;

public sealed class PythonRecognitionBridge : IPythonRecognitionBridge
{
    private readonly RecognitionOptions _options;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PythonRecognitionBridge> _logger;

    public PythonRecognitionBridge(
        IOptions<RecognitionOptions> options,
        IWebHostEnvironment environment,
        ILogger<PythonRecognitionBridge> logger)
    {
        _options = options.Value;
        _environment = environment;
        _logger = logger;
    }

    public async Task<RecognitionResultDto> PredictAsync(string fileName, Stream imageStream, CancellationToken cancellationToken)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}");

        try
        {
            await using (var target = File.Create(tempFilePath))
            {
                await imageStream.CopyToAsync(target, cancellationToken);
            }
            var contentRoot = _environment.ContentRootPath;
            var bridgePath = Path.GetFullPath(Path.Combine(contentRoot, _options.BridgeScriptPath));
            if (!File.Exists(bridgePath))
            {
                return new RecognitionResultDto(fileName, [], null, $"Recognition bridge script not found: {bridgePath}");
            }

            var workingDirectory = Directory.GetParent(contentRoot)?.FullName ?? contentRoot;
            var pythonExecutable = ResolvePythonExecutable(contentRoot, workingDirectory);

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = pythonExecutable,
                Arguments = $"\"{bridgePath}\" \"{tempFilePath}\"",
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (!string.IsNullOrWhiteSpace(_options.ModelPath))
            {
                process.StartInfo.Environment["ORIGAMI_MODEL_PATH"] = ResolveModelPath(contentRoot, workingDirectory);
            }

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (_, args) =>
            {
                if (args.Data is not null)
                {
                    outputBuilder.AppendLine(args.Data);
                }
            };

            process.ErrorDataReceived += (_, args) =>
            {
                if (args.Data is not null)
                {
                    errorBuilder.AppendLine(args.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            await process.WaitForExitAsync(linkedCts.Token);

            var output = outputBuilder.ToString().Trim();
            var error = errorBuilder.ToString().Trim();

            if (process.ExitCode != 0)
            {
                _logger.LogWarning("Python recognition bridge failed: {Error}", error);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    var parsed = ParseOutput(fileName, output);
                    if (!string.IsNullOrWhiteSpace(parsed.Error))
                    {
                        return parsed;
                    }
                }

                return new RecognitionResultDto(fileName, [], output, SummarizeBridgeFailure(error));
            }

            return ParseOutput(fileName, output);
        }
        catch (OperationCanceledException)
        {
            return new RecognitionResultDto(fileName, [], null, "Recognition timed out.");
        }
        finally
        {
            try
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Could not delete temporary file {TempFilePath}", tempFilePath);
            }
        }
    }

    private static RecognitionResultDto ParseOutput(string fileName, string output)
    {
        if (string.IsNullOrWhiteSpace(output))
        {
            return new RecognitionResultDto(fileName, [], null, "Recognition bridge returned no output.");
        }

        try
        {
            using var document = JsonDocument.Parse(output);
            var root = document.RootElement;
            var predictions = new List<RecognitionPredictionDto>();

            if (root.TryGetProperty("predictions", out var predictionsElement))
            {
                foreach (var item in predictionsElement.EnumerateArray())
                {
                    predictions.Add(new RecognitionPredictionDto(
                        item.TryGetProperty("class_index", out var indexValue) ? indexValue.GetInt32() : -1,
                        item.TryGetProperty("label", out var labelValue) ? labelValue.GetString() ?? string.Empty : string.Empty,
                        item.TryGetProperty("confidence", out var confidenceValue) ? confidenceValue.GetDouble() : 0));
                }
            }

            return new RecognitionResultDto(
                fileName,
                predictions,
                output,
                root.TryGetProperty("error", out var errorValue) ? errorValue.GetString() : null);
        }
        catch (JsonException)
        {
            return new RecognitionResultDto(fileName, [], output, "Recognition bridge returned non-JSON output.");
        }
    }

    private static string SummarizeBridgeFailure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            return "Recognition bridge failed.";
        }

        if (error.Contains("ModuleNotFoundError: No module named 'cv2'", StringComparison.Ordinal))
        {
            return "Recognition dependency missing: Python package 'opencv-python' (module 'cv2') is not installed.";
        }

        var lines = error
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return lines.LastOrDefault() ?? "Recognition bridge failed.";
    }

    private string ResolvePythonExecutable(string contentRoot, string workingDirectory)
    {
        if (Path.IsPathRooted(_options.PythonExecutable))
        {
            return _options.PythonExecutable;
        }

        var candidates = new[]
        {
            Path.Combine(contentRoot, _options.PythonExecutable),
            Path.Combine(workingDirectory, _options.PythonExecutable)
        };

        return candidates.FirstOrDefault(File.Exists) ?? _options.PythonExecutable;
    }

    private string ResolveModelPath(string contentRoot, string workingDirectory)
    {
        if (string.IsNullOrWhiteSpace(_options.ModelPath))
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(_options.ModelPath))
        {
            return _options.ModelPath;
        }

        var candidates = new[]
        {
            Path.Combine(contentRoot, _options.ModelPath),
            Path.Combine(workingDirectory, _options.ModelPath)
        };

        return candidates.FirstOrDefault(File.Exists) ?? Path.Combine(workingDirectory, _options.ModelPath);
    }
}
