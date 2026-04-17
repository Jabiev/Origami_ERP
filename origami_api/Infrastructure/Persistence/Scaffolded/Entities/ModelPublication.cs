using System;
using System.Collections.Generic;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

public partial class ModelPublication
{
    public Guid ModelId { get; set; }

    public Guid PublicationId { get; set; }

    public string? InstructionType { get; set; }

    public string? InstructionUrl { get; set; }

    public int? PageNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Model Model { get; set; } = null!;

    public virtual Publication Publication { get; set; } = null!;
}
