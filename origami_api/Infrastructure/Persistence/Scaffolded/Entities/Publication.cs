using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class Publication
{
    public Guid PublicationId { get; set; }

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? Year { get; set; }

    public string? Publisher { get; set; }

    public string? Language { get; set; }

    public string? Isbn { get; set; }

    public string? Url { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ModelPublication> ModelPublications { get; set; } = new List<ModelPublication>();
}
