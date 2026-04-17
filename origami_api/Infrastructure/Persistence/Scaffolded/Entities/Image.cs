using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class Image
{
    public Guid ImageId { get; set; }

    public Guid ModelId { get; set; }

    public string? Url { get; set; }

    public string? LocalPath { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? Angle { get; set; }

    public bool? IsPrimary { get; set; }

    public string? Hash { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CloudinaryUrl { get; set; }

    public virtual Model Model { get; set; } = null!;
}
