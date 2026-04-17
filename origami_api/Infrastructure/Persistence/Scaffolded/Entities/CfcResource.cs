using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class CfcResource
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Url { get; set; }

    public string? UpdatedDate { get; set; }

    public string? PostedOn { get; set; }

    public string? Summary { get; set; }

    public string? Body { get; set; }

    public string? ResourceLinks { get; set; }

    public DateTime? ScrapedAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}
