using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class CfcDiagram
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Url { get; set; }

    public string? Creator { get; set; }

    public string? Language { get; set; }

    public string? Description { get; set; }

    public string? Difficulty { get; set; }

    public string? PaperSize { get; set; }

    public string? Category { get; set; }

    public string? ImageUrl { get; set; }

    public string? CloudinaryUrl { get; set; }

    public string? Downloads { get; set; }

    public DateTime? ScrapedAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}
