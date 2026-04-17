namespace Origami.Api.Modules.Common;

public sealed record PagedResult<T>(int Count, IReadOnlyList<T> Items);
