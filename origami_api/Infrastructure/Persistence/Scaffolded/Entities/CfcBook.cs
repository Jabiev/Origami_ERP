using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class CfcBook
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Url { get; set; }

    public string? Author { get; set; }

    public string? PublishedDate { get; set; }

    public string? ImageUrl { get; set; }

    public string? CloudinaryUrl { get; set; }

    public DateTime? ScrapedAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}
