using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class Creator
{
    public Guid CreatorId { get; set; }

    public string NameOriginal { get; set; } = null!;

    public string NameNormalized { get; set; } = null!;

    public string? Language { get; set; }

    public string? Country { get; set; }

    public int? BirthYear { get; set; }

    public int? DeathYear { get; set; }

    public string? Biography { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CreatorAlias> CreatorAliases { get; set; } = new List<CreatorAlias>();

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
