using System;
using AutoMapper;
using Emmy.Data.Entities.Discord;

namespace Emmy.Services.Discord.Role.Models
{
    public record UserRoleDto(
        Guid Id,
        long RoleId,
        bool IsPersonal,
        DateTimeOffset? Expiration);

    public class UserRoleProfile : Profile
    {
        public UserRoleProfile() => CreateMap<UserRole, UserRoleDto>();
    }
}