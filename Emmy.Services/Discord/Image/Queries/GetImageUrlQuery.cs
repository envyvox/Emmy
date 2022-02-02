using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.Image.Queries
{
    public record GetImageUrlQuery(Data.Enums.Image Type) : IRequest<string>;

    public class GetImageUrlHandler : IRequestHandler<GetImageUrlQuery, string>
    {
        private readonly AppDbContext _db;

        public GetImageUrlHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<string> Handle(GetImageUrlQuery request, CancellationToken ct)
        {
            var requestedImageUrl = await _db.Images
                .OrderByRandom()
                .Where(x => x.Type == request.Type)
                .Take(1)
                .Select(x => x.Url)
                .SingleOrDefaultAsync();

            if (requestedImageUrl is not null) return requestedImageUrl;

            var placeholderUrl = await _db.Images
                .AsQueryable()
                .Where(x => x.Type == Data.Enums.Image.Placeholder)
                .Select(x => x.Url)
                .SingleOrDefaultAsync();

            if (placeholderUrl is null)
            {
                throw new Exception(
                    $"there is no urls for image {request.Type.ToString()} or even {Data.Enums.Image.Placeholder.ToString()} in db");
            }

            return placeholderUrl;
        }
    }
}