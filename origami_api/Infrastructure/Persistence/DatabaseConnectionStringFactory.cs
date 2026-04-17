using Npgsql;
using Origami.Api.Configuration;

namespace Origami.Api.Infrastructure.Persistence;

public static class DatabaseConnectionStringFactory
{
    public static string Build(DatabaseOptions options)
    {
        var fromEnvironment =
            Environment.GetEnvironmentVariable("ORIGAMI_DB_CONNECTION_STRING")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__OrigamiDb");

        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return options.ConnectionString;
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = options.Host,
            Port = options.Port,
            Database = options.Database,
            Username = options.Username,
            Password = options.Password,
            SslMode = Enum.TryParse<Npgsql.SslMode>(options.SslMode, true, out var sslMode)
                ? sslMode
                : Npgsql.SslMode.Require
        };

        return builder.ConnectionString;
    }
}
