using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class CreatorAlias
{
    public Guid AliasId { get; set; }

    public Guid CreatorId { get; set; }

    public string Alias { get; set; } = null!;

    public string AliasNormalized { get; set; } = null!;

    public string? Language { get; set; }

    public string? Source { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Creator Creator { get; set; } = null!;
}
