namespace Origami.Api.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string? ConnectionString { get; set; }

    public string Host { get; set; } = "";

    public int Port { get; set; } = 5432;

    public string Database { get; set; } = "";

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public string SslMode { get; set; } = "Require";
}
