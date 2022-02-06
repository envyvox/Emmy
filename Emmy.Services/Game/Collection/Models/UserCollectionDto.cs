using System;
using AutoMapper;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Collection.Models
{
    public record UserCollectionDto(
        CollectionCategory Category,
        Guid ItemId);

    public class UserCollectionProfile : Profile
    {
        public UserCollectionProfile() => CreateMap<UserCollection, UserCollectionDto>();
    }
}