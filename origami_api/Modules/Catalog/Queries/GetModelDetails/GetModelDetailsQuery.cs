using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Catalog.Contracts;

namespace Origami.Api.Modules.Catalog.Queries.GetModelDetails;

public sealed record GetModelDetailsQuery(Guid ModelId) : IRequest<CatalogModelDetailsDto?>;

public sealed class GetModelDetailsHandler : IRequestHandler<GetModelDetailsQuery, CatalogModelDetailsDto?>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetModelDetailsHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CatalogModelDetailsDto?> Handle(GetModelDetailsQuery request, CancellationToken cancellationToken)
    {
        var model = await _dbContext.Models
            .AsNoTracking()
            .Include(x => x.Creator)
            .Include(x => x.Images)
            .Include(x => x.ModelPublications)
                .ThenInclude(x => x.Publication)
            .FirstOrDefaultAsync(x => x.ModelId == request.ModelId, cancellationToken);

        if (model is null)
        {
            return null;
        }

        return new CatalogModelDetailsDto(
            model.ModelId,
            model.ModelNameOriginal,
            model.ModelNameNormalized,
            model.Difficulty,
            model.PaperShape,
            model.PaperToModelRatio,
            model.RecommendedPaperSize,
            model.Pieces,
            model.YearCreated,
            model.IsAbstract,
            model.UsesCutting,
            model.UsesGlue,
            model.SourceUrl,
            model.CreatorId,
            model.Creator?.NameOriginal,
            model.Images
                .OrderByDescending(x => x.IsPrimary)
                .Select(x => new CatalogImageDto(x.ImageId, x.Url, x.CloudinaryUrl, x.IsPrimary ?? false, x.Width, x.Height, x.Angle))
                .ToList(),
            model.ModelPublications
                .Select(x => new CatalogPublicationDto(
                    x.PublicationId,
                    x.Publication.Title,
                    x.Publication.Type,
                    x.Publication.Publisher,
                    x.Publication.Year,
                    x.Publication.Url,
                    x.InstructionType,
                    x.InstructionUrl,
                    x.PageNumber))
                .ToList());
    }
}
