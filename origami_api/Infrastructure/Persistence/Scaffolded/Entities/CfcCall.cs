using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class CfcCall
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Url { get; set; }

    public string? PostedOn { get; set; }

    public string? SubmissionDeadline { get; set; }

    public string? Summary { get; set; }

    public DateTime? ScrapedAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}
