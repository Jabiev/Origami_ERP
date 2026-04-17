namespace Origami.Api.Configuration;

public sealed class RecognitionOptions
{
    public const string SectionName = "Recognition";

    public string PythonExecutable { get; set; } = "..\\.venv\\Scripts\\python.exe";

    public string BridgeScriptPath { get; set; } = "..\\ai\\api_predict.py";

    public string? ModelPath { get; set; }

    public int TimeoutSeconds { get; set; } = 120;
}
