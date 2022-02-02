﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record UpdateUserPremiumCommand(long UserId, bool IsPremium) : IRequest;

    public class UpdateUserPremiumHandler : IRequestHandler<UpdateUserPremiumCommand>
    {
        private readonly ILogger<UpdateUserPremiumHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserPremiumHandler(
            DbContextOptions options,
            ILogger<UpdateUserPremiumHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserPremiumCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.IsPremium = request.IsPremium;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} premium status to {IsPremium}",
                request.UserId, request.IsPremium);

            return Unit.Value;
        }
    }
}