using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums.Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.World.Queries
{
    public record CheckModuleEnabledQuery(CommandModule Module) : IRequest<bool>;

    public class CheckModuleEnabledHandler : IRequestHandler<CheckModuleEnabledQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckModuleEnabledHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckModuleEnabledQuery request, CancellationToken ct)
        {
            return await _db.WorldModules
                .AsQueryable()
                .Where(x => x.Module == request.Module)
                .Select(x => x.IsEnabled)
                .SingleOrDefaultAsync();
        }
    }
}