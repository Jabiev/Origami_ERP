using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Analytics.Contracts;

namespace Origami.Api.Modules.Analytics.Queries.GetCreatorAnalytics;

public sealed record GetCreatorAnalyticsQuery() : IRequest<CreatorAnalyticsDto>;

public sealed class GetCreatorAnalyticsHandler : IRequestHandler<GetCreatorAnalyticsQuery, CreatorAnalyticsDto>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetCreatorAnalyticsHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreatorAnalyticsDto> Handle(GetCreatorAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var topCreators = await _dbContext.Creators
            .AsNoTracking()
            .Select(x => new
            {
                Name = x.NameOriginal,
                Count = x.Models.Count
            })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Name)
            .Take(20)
            .Select(x => new NamedCountDto(x.Name, x.Count))
            .ToListAsync(cancellationToken);

        var topCountries = await _dbContext.Creators
            .AsNoTracking()
            .Where(x => x.Country != null && x.Country != "")
            .Select(x => new
            {
                Country = x.Country!,
                ModelCount = x.Models.Count
            })
            .GroupBy(x => x.Country)
            .Select(group => new
            {
                Country = group.Key,
                Count = group.Sum(x => x.ModelCount)
            })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Country)
            .Take(15)
            .Select(x => new CreatorCountryDto(x.Country, x.Count))
            .ToListAsync(cancellationToken);

        var productivitySource = await _dbContext.Creators
            .AsNoTracking()
            .Select(x => x.Models.Count)
            .Where(x => x > 0)
            .ToListAsync(cancellationToken);

        var productivity = productivitySource
            .GroupBy(Bucketize)
            .Select(group => new CreatorProductivityDto(group.Key, group.Count()))
            .OrderBy(x => BucketOrder(x.Bucket))
            .ToList();

        return new CreatorAnalyticsDto(topCreators, topCountries, productivity);
    }

    private static string Bucketize(int count)
    {
        if (count <= 5) return "1-5";
        if (count <= 10) return "6-10";
        if (count <= 25) return "11-25";
        if (count <= 50) return "26-50";
        if (count <= 100) return "51-100";
        return "100+";
    }

    private static int BucketOrder(string bucket) => bucket switch
    {
        "1-5" => 1,
        "6-10" => 2,
        "11-25" => 3,
        "26-50" => 4,
        "51-100" => 5,
        "100+" => 6,
        _ => 99
    };
}
