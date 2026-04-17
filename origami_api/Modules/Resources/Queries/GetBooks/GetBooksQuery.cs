using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Modules.Common;
using Origami.Api.Modules.Resources.Contracts;

namespace Origami.Api.Modules.Resources.Queries.GetBooks;

public sealed record GetBooksQuery(int Limit = 20) : IRequest<PagedResult<BookDto>>;

public sealed class GetBooksHandler : IRequestHandler<GetBooksQuery, PagedResult<BookDto>>
{
    private readonly OrigamiScaffoldDbContext _dbContext;

    public GetBooksHandler(OrigamiScaffoldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);
        var items = await _dbContext.CfcBooks
            .AsNoTracking()
            .OrderByDescending(x => x.ScrapedAt)
            .Take(limit)
            .Select(x => new BookDto(x.Id, x.Title, x.Author, x.PublishedDate, x.Url, x.ImageUrl, x.CloudinaryUrl))
            .ToListAsync(cancellationToken);

        return new PagedResult<BookDto>(items.Count, items);
    }
}
