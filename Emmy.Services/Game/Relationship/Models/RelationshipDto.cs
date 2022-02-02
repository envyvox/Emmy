using System;
using AutoMapper;
using Emmy.Services.Game.User.Models;

namespace Emmy.Services.Game.Relationship.Models
{
    public record RelationshipDto(
        Guid Id,
        UserDto User1,
        UserDto User2,
        DateTimeOffset CreatedAt);

    public class RelationshipProfile : Profile
    {
        public RelationshipProfile() => CreateMap<Data.Entities.Relationship, RelationshipDto>();
    }
}