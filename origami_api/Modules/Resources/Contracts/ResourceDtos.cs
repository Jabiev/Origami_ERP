namespace Origami.Api.Modules.Resources.Contracts;

public sealed record BookDto(Guid Id, string Title, string? Author, string? PublishedDate, string? Url, string? ImageUrl, string? CloudinaryUrl);

public sealed record DiagramDto(Guid Id, string Title, string? Creator, string? Category, string? Difficulty, string? Url, string? CloudinaryUrl);

public sealed record ArticleDto(Guid Id, string Title, string? Summary, string? Url, string? PostedOn, string? UpdatedDate);

public sealed record CallDto(Guid Id, string Title, string? Summary, string? Url, string? PostedOn, string? SubmissionDeadline);
