using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Catalog.Contracts;
using Origami.Api.Modules.Common;

namespace Origami.Api.Modules.Catalog.Queries.SearchModels;

public sealed record SearchModelsQuery(
    string? Query,
    Guid? CreatorId,
    string? Difficulty,
    string? PaperShape,
    bool? UsesCutting,
    bool? UsesGlue,
    int Limit = 20) : IRequest<PagedResult<CatalogModelSummaryDto>>;

public sealed class SearchModelsHandler : IRequestHandler<SearchModelsQuery, PagedResult<CatalogModelSummaryDto>>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public SearchModelsHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CatalogModelSummaryDto>> Handle(SearchModelsQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);
        var query = _dbContext.Models
            .AsNoTracking()
            .Include(x => x.Creator)
            .Include(x => x.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var search = request.Query.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.ModelNameOriginal, $"%{search}%") ||
                (x.ModelNameNormalized != null && EF.Functions.ILike(x.ModelNameNormalized, $"%{search}%")) ||
                (x.Creator != null && EF.Functions.ILike(x.Creator.NameOriginal, $"%{search}%")) ||
                (x.Creator != null && x.Creator.NameNormalized != null && EF.Functions.ILike(x.Creator.NameNormalized, $"%{search}%")));
        }

        if (request.CreatorId.HasValue)
        {
            query = query.Where(x => x.CreatorId == request.CreatorId);
        }

        if (!string.IsNullOrWhiteSpace(request.Difficulty))
        {
            var difficulty = request.Difficulty.Trim();
            query = query.Where(x => x.Difficulty != null && x.Difficulty == difficulty);
        }

        if (!string.IsNullOrWhiteSpace(request.PaperShape))
        {
            query = query.Where(x => x.PaperShape != null && EF.Functions.ILike(x.PaperShape, $"%{request.PaperShape.Trim()}%"));
        }

        if (request.UsesCutting.HasValue)
        {
            query = query.Where(x => x.UsesCutting == request.UsesCutting);
        }

        if (request.UsesGlue.HasValue)
        {
            query = query.Where(x => x.UsesGlue == request.UsesGlue);
        }

        var items = await query
            .OrderBy(x => x.ModelNameOriginal)
            .Take(limit)
            .Select(x => new CatalogModelSummaryDto(
                x.ModelId,
                x.ModelNameOriginal,
                x.ModelNameNormalized,
                x.Difficulty,
                x.PaperShape,
                x.Pieces,
                x.UsesCutting,
                x.UsesGlue,
                x.SourceUrl,
                x.CreatorId,
                x.Creator != null ? x.Creator.NameOriginal : null,
                x.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.CloudinaryUrl ?? i.Url).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResult<CatalogModelSummaryDto>(items.Count, items);
    }
}
