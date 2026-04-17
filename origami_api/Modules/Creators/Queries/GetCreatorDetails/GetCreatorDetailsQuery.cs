using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Creators.Contracts;

namespace Origami.Api.Modules.Creators.Queries.GetCreatorDetails;

public sealed record GetCreatorDetailsQuery(Guid CreatorId) : IRequest<CreatorDetailsDto?>;

public sealed class GetCreatorDetailsHandler : IRequestHandler<GetCreatorDetailsQuery, CreatorDetailsDto?>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetCreatorDetailsHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreatorDetailsDto?> Handle(GetCreatorDetailsQuery request, CancellationToken cancellationToken)
    {
        var creator = await _dbContext.Creators
            .AsNoTracking()
            .Include(x => x.CreatorAliases)
            .Include(x => x.Models)
                .ThenInclude(x => x.Images)
            .FirstOrDefaultAsync(x => x.CreatorId == request.CreatorId, cancellationToken);

        if (creator is null)
        {
            return null;
        }

        return new CreatorDetailsDto(
            creator.CreatorId,
            creator.NameOriginal,
            creator.NameNormalized,
            creator.Biography,
            creator.Country,
            creator.Language,
            creator.BirthYear,
            creator.DeathYear,
            creator.CreatorAliases
                .OrderBy(x => x.Alias)
                .Select(x => new CreatorAliasDto(x.AliasId, x.Alias, x.Language, x.Source))
                .ToList(),
            creator.Models
                .OrderBy(x => x.ModelNameOriginal)
                .Take(20)
                .Select(x => new CreatorModelDto(
                    x.ModelId,
                    x.ModelNameOriginal,
                    x.Difficulty,
                    x.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.CloudinaryUrl ?? i.Url).FirstOrDefault()))
                .ToList());
    }
}
