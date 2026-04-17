using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class Model
{
    public Guid ModelId { get; set; }

    public Guid CreatorId { get; set; }

    public string ModelNameOriginal { get; set; } = null!;

    public string ModelNameNormalized { get; set; } = null!;

    public int? YearCreated { get; set; }

    public string? PaperShape { get; set; }

    public int? Pieces { get; set; }

    public bool UsesCutting { get; set; }

    public bool UsesGlue { get; set; }

    public string? RecommendedPaperSize { get; set; }

    public string? PaperToModelRatio { get; set; }

    public string? Difficulty { get; set; }

    public bool? IsAbstract { get; set; }

    public string? SourceUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Creator Creator { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<ModelPublication> ModelPublications { get; set; } = new List<ModelPublication>();
}
