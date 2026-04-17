using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class OrcModel
{
    public long Id { get; set; }

    public string ModelName { get; set; } = null!;

    public string? ModelNameBase { get; set; }

    public int? VariantIndex { get; set; }

    public string? CreatorRaw { get; set; }

    public string? CreatorExpanded { get; set; }

    public string? CreatorType { get; set; }

    public string? Category { get; set; }

    public string? Subcategory { get; set; }

    public string SourcePageUrl { get; set; } = null!;

    public string? PageTitle { get; set; }

    public DateTime? PageModified { get; set; }

    public DateTime? SitemapLastmod { get; set; }

    public string? DiagramUrl { get; set; }

    public string? DiagramType { get; set; }

    public bool? DiagramIsHostedOnOrc { get; set; }

    public bool? DiagramIsArchived { get; set; }

    public bool? IsDollarBill { get; set; }

    public string? ImageUrl { get; set; }

    public string? CloudinaryUrl { get; set; }

    public DateTime ScrapedAt { get; set; }
}
